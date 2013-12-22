using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Q3
{
    class CRUDManager
    {
        string connectionSting = ConfigurationManager.ConnectionStrings["Q3.Properties.Settings.QuestionsDbConnectionString"].ConnectionString;

        #region CREATE
        #endregion

        #region RETRIEVE

        public User GetAdminLoginDetails(string UserName)
        {
            User AdminInfo = null;
            string commandText = "SELECT * FROM AdminInfo WHERE (UserName = @userName)";

            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@userName", UserName);
                        SqlCeDataReader reader = command.ExecuteReader();

                        AdminInfo = new User();

                        while (reader.Read())
                        {
                            AdminInfo.UID = new Guid(reader[0].ToString());
                            AdminInfo.Name = reader[1].ToString();
                            AdminInfo.UserName = reader[2].ToString();
                            AdminInfo.Password = reader[3].ToString();
                            AdminInfo.IDNumber = reader[4].ToString();

                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            return AdminInfo;
        }

        public List<ImageModel> GetAllImageFromDB(Guid QID, SqlCeConnection connection)
        {
            if (QID.Equals(Guid.Empty)) return null;
            
            List<ImageModel> images = new List<ImageModel>();

            string commandText = "SELECT * FROM Images WHERE (QID = @qID)";

            //FileStream FS1 = new FileStream("image.jpg", FileMode.Create);
            //FS1.Write(blob, 0, blob.Length);

            //FS1.Close();

            //FS1 = null;

            //index++; 
            //using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            //{
                using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
                {
                    try
                    {
                        //connection.Open();
                        command.Parameters.AddWithValue("@qID", QID);
                        SqlCeDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            ImageModel imageM = new ImageModel();
                            imageM.ImageID = new Guid(reader[0].ToString());
                            imageM.QID = QID;
                            imageM.Image = (byte[])reader[2];
                         
                            images.Add(imageM);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                //}
            }
            return images;
        }

        public List<QuestionModel> GetAllQuestions() 
        {
            List<QuestionModel> QList = new List<QuestionModel>();
            
            string commandText = "SELECT QID, QBody, MultipleAnswers, Images FROM Questions";

            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlCeDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            QuestionModel q = new QuestionModel();
                            q.QID = new Guid(reader[0].ToString());
                            q.QBody = reader[1].ToString();
                            q.HasMultipleAnswers = (bool)reader[2];
                            q.HasImages = (bool)reader[3];
                            q.Answers = GetAnswers(q.QID, connection);
                            q.Images = GetAllImageFromDB(q.QID, connection);

                            QList.Add(q);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            return QList;
        }

        private List<AnswersModel> GetAnswers(Guid QID, SqlCeConnection connection)
        {
            if (QID.Equals(Guid.Empty)) return null;

            List<AnswersModel> Answers = new List<AnswersModel>();
            string commandText = "SELECT AID, QID, ABody, IsCorrect FROM Answers WHERE (QID = @qID)";
            using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
            {
                try
                {
                    command.Parameters.AddWithValue("@qID", QID);
                    SqlCeDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        AnswersModel a = new AnswersModel();
                        a.AID = new Guid(reader[0].ToString());
                        a.QID = QID;
                        a.AnswerBody = reader[2].ToString();
                        a.IsCorrectAnswer = (bool)reader[3];
                        //q.Add(imageM);
                        Answers.Add(a);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            return Answers;
        }

        #endregion

        #region UPDATE

        public bool UpdateQuestion(QuestionModel Question)
        {
            if (Question == null) return false;
            bool success = false;
            string command1Text = "UPDATE Questions SET QBody = @qBody, MultipleAnswers = @multipleAnswers," +
                " Images = @hasImages WHERE(QID = @qID)";
            string command2Text = "INSERT INTO Answers(AID, QID, ABody, IsCorrect)" +
                "VALUES(@aID, @qID, @aBody, @isCorrect)";
            string command3Text = "INSERT INTO Images(IID, QID, Image)" +
                "VALUES(@iID, @qID, @image)";
            string command4Text = "DELETE FROM Answers WHERE QID=@qID";
            string command5Text = "DELETE FROM Images WHERE QID=@qID";
            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                connection.Open();
                using (IDbTransaction tx = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    using (SqlCeCommand command1 = new SqlCeCommand(command1Text, connection))
                    {
                        try
                        {
                            command1.Parameters.AddWithValue("@qID", Question.QID);
                            command1.Parameters.AddWithValue("@QBody", Question.QBody);
                            if (Question.HasMultipleAnswers)
                            {
                                command1.Parameters.AddWithValue("@multipleAnswers", 1);
                            }
                            else command1.Parameters.AddWithValue("@multipleAnswers", 0);
                            if (Question.HasImages)
                            {
                                command1.Parameters.AddWithValue("@hasImages", 1);
                            }
                            else command1.Parameters.AddWithValue("@hasImages", 0);

                            command1.ExecuteNonQuery();
                            success = true;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            success = false;
                        }
                    }
                    DeleteAnswersFromDb(Question.QID, command4Text, connection);

                    for (int i = 0; i < Question.Answers.Count; i++)
                    {
                        using (SqlCeCommand command2 = new SqlCeCommand(command2Text, connection))
                        {
                            try
                            {
                                command2.Parameters.AddWithValue("@aID", Question.Answers[i].AID);
                                command2.Parameters.AddWithValue("@qID", Question.Answers[i].QID);
                                command2.Parameters.AddWithValue("@aBody", Question.Answers[i].AnswerBody);
                                if (Question.Answers[i].IsCorrectAnswer)
                                {
                                    command2.Parameters.AddWithValue("@isCorrect", 1);
                                }
                                else command2.Parameters.AddWithValue("@isCorrect", 0);

                                command2.ExecuteNonQuery();
                                success = true;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                                success = false;
                            }
                        }
                    }
                    DeleteImagesFromDB(Question.QID, command5Text, connection);

                    for (int i = 0; i < Question.Images.Count; i++)
                    {
                        using (SqlCeCommand command3 = new SqlCeCommand(command3Text, connection))
                        {
                            try
                            {
                                command3.Parameters.AddWithValue("@iID", Question.Images[i].ImageID);
                                command3.Parameters.AddWithValue("@qID", Question.Images[i].QID);

                                SqlCeParameter picparameter = new SqlCeParameter();
                                picparameter.SqlDbType = SqlDbType.Image;
                                picparameter.ParameterName = "image";
                                picparameter.Value = Question.Images[i].Image;
                                command3.Parameters.Add(picparameter);

                                command3.ExecuteNonQuery();
                                success = true;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                                success = false;
                            }
                        }
                    }
                }
            }


            return success;
        }

        #endregion

        #region DELETE

        private static void DeleteAnswersFromDb(Guid QuestionID, string commandText, SqlCeConnection connection)
        {
            using (SqlCeCommand command4 = new SqlCeCommand(commandText, connection))
            {
                command4.Parameters.AddWithValue("@qID", QuestionID);
                command4.ExecuteNonQuery();
            }
        }

        private static void DeleteImagesFromDB(Guid QuestionID, string commandText, SqlCeConnection connection)
        {
            using (SqlCeCommand command5 = new SqlCeCommand(commandText, connection))
            {
                command5.Parameters.AddWithValue("@qID", QuestionID);
                command5.ExecuteNonQuery();
            }
        }

        private static void DeleteQuestionFromDB(Guid QuestionID, string commandText, SqlCeConnection connection)
        {
            using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@qID", QuestionID);
                command.ExecuteNonQuery();
            }
        }

        internal bool DeleteGivenQuestion(Guid QuestionID)
        {
            string commandText = "DELETE FROM Questions WHERE QID=@qID";
            string command2Text = "DELETE FROM Answers WHERE QID=@qID";
            string command3Text = "DELETE FROM Images WHERE QID=@qID";

            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                connection.Open();
                try
                {
                    DeleteQuestionFromDB(QuestionID, commandText, connection);
                    DeleteAnswersFromDb(QuestionID, command2Text, connection);
                    DeleteImagesFromDB(QuestionID, command3Text, connection);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return false;
                }
                
            }

            return true;
        }

        #endregion

        #region INSERT

        public bool AddNewUser(String Name, String NICNumber) 
        {
            string commandText = "INSERT INTO UserInfo(UserID, NICNumber, UserName, Marks, Attempt)"+
                "VALUES(@userID, @nICNumber, @userName, @marks, @attempt)";

            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@userID", Guid.NewGuid());
                        command.Parameters.AddWithValue("@nICNumber", NICNumber);
                        command.Parameters.AddWithValue("@userName", Name);
                        command.Parameters.AddWithValue("@marks", 0);
                        command.Parameters.AddWithValue("@attempt", 0);
                        command.Parameters.AddWithValue("@password", "");
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        return false; 
                    }
                }
            }

            return true;
        }

        public bool AddNewAdmin(string UserName, string Password)
        {
            string commandText = "INSERT INTO AdminInfo(AID, Name, UserName, Password, NICNumber)" +
                "VALUES(@aID, @name, @userName, @password, @nicNumber)";

            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@aID", Guid.NewGuid());
                        command.Parameters.AddWithValue("@name", "");
                        command.Parameters.AddWithValue("@userName", UserName);
                        command.Parameters.AddWithValue("@password", Password);
                        command.Parameters.AddWithValue("@nicNumber", "");
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        return false;
                    }
                }
            }

            return true;
        }

        public bool AddNewQuestion(QuestionModel Question) 
        {
            if (Question == null) return false;
            bool success = false;

            string command1Text = "INSERT INTO Questions(QID, QBody, MultipleAnswers, Images)" +
                "VALUES(@qID, @qBody, @hasMultiAnswers, @hasImages)";
            string command2Text = "INSERT INTO Answers(AID, QID, ABody, IsCorrect)" +
                "VALUES(@aID, @qID, @aBody, @isCorrect)";
            string command3Text = "INSERT INTO Images(IID, QID, Image)" +
                "VALUES(@iID, @qID, @image)";
            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                connection.Open();
                using (IDbTransaction tx = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    using (SqlCeCommand command1 = new SqlCeCommand(command1Text, connection))
                    {
                        try
                        {
                            command1.Parameters.AddWithValue("@qID", Question.QID);
                            command1.Parameters.AddWithValue("@QBody", Question.QBody);
                            if (Question.HasMultipleAnswers)
                            {
                                command1.Parameters.AddWithValue("@hasMultiAnswers", 1);
                            }
                            else command1.Parameters.AddWithValue("@hasMultiAnswers", 0);
                            if (Question.HasImages)
                            {
                                command1.Parameters.AddWithValue("@hasImages", 1);
                            }
                            else command1.Parameters.AddWithValue("@hasImages", 0);

                            command1.ExecuteNonQuery();
                            success = true;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            success = false;
                        }
                    }
                    for (int i = 0; i < Question.Answers.Count; i++)
                    {
                        using (SqlCeCommand command2 = new SqlCeCommand(command2Text, connection))
                        {
                            try
                            {
                                command2.Parameters.AddWithValue("@aID", Question.Answers[i].AID);
                                command2.Parameters.AddWithValue("@qID", Question.Answers[i].QID);
                                command2.Parameters.AddWithValue("@aBody", Question.Answers[i].AnswerBody);
                                if (Question.Answers[i].IsCorrectAnswer)
                                {
                                    command2.Parameters.AddWithValue("@isCorrect", 1);
                                }
                                else command2.Parameters.AddWithValue("@isCorrect", 0);
                                command2.ExecuteNonQuery();
                                success = true;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                                success = false;
                            }
                        }
                    }
                    for (int i = 0; i < Question.Images.Count; i++)
                    {
                        using (SqlCeCommand command3 = new SqlCeCommand(command3Text, connection))
                        {
                            try
                            {
                                command3.Parameters.AddWithValue("@iID", Question.Images[i].ImageID);
                                command3.Parameters.AddWithValue("@qID", Question.Images[i].QID);
                                
                                SqlCeParameter picparameter = new SqlCeParameter();
                                picparameter.SqlDbType = SqlDbType.Image;
                                picparameter.ParameterName = "image";
                                picparameter.Value = Question.Images[i].Image;
                                command3.Parameters.Add(picparameter);

                                command3.ExecuteNonQuery();
                                success = true;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                                success = false;
                            }
                        }
                    }
                }
            }


            return success;
        }

        public bool InsertImageToDB(Guid QID, byte[] Image) 
        {
            if (QID.Equals(Guid.Empty) || Image == null) return false;
            
            string commandText = "INSERT INTO Images (IID, QID, Image) VALUES (@iID, @qID, @img)";
            using (SqlCeConnection connection = new SqlCeConnection(connectionSting))
            {
                using (SqlCeCommand command = new SqlCeCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@iID", Guid.NewGuid());
                        command.Parameters.AddWithValue("@qID", QID);
                        
                        SqlCeParameter picparameter = new SqlCeParameter();
                        picparameter.SqlDbType = SqlDbType.Image;
                        picparameter.ParameterName = "img";
                        picparameter.Value = Image;

                        command.Parameters.Add(picparameter);
                        command.ExecuteNonQuery();

                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        return false;
                    }
                }
            }
        }
        
        #endregion

        public int GetNumberOfQuestions()
        {
            return 25;
        }

        internal TimeSpan GetTimeSpan()
        {
            throw new NotImplementedException();
        }
    }
}
