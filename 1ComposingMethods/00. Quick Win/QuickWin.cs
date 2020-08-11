using System;
using System.Linq;
using System.Net.Mail;

namespace ComposingMethods.QuickWin
{
    public class ModelState
    {
        public static void AddModelError(string key, string errorMessage)
        { }
    }

   

    public class QuickWin
    {
        private readonly SmtpClient _mailEngine = new SmtpClient();

        public ActionResult ImportStudents(string csv)
        {
            foreach (string[] d in csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(line => line.Split(new[] { '|' })))
            {
                int id;
                if (int.TryParse(d[0], out id))
                {
                    if (StudentRepository.GetById(id) != null)
                    {
                        return StudentAlreadyExistsError(id);
                    }

                    var student = CreateStudent(id, d);

                    string password = PasswordGenerator.CreateRandomPassword();
                    student.Password = Crypto.HashPassword(password);

                    SendMailWithPassword(student);


                    StudentRepository.Add(student);
                }
                else
                {
                    ModelState.AddModelError("", $"Student id {id} is not a number.");
                    return View();
                }
            }


            return View();
        }

        private static Student CreateStudent(int id, string[] d)
        {
            var student = new Student
            {
                Id = id,
                FirstName = d[1],
                LastName = d[2],
                Email = d[3].ToLower(),
                ClassId = int.Parse(d[4]),
                Telephone = d[5],
                StreetName = d[6],
                StreetNumber = int.Parse(d[7]),
                ZipCode = d[8],
                City = d[9],
            };
            return student;
        }

        private ActionResult StudentAlreadyExistsError(int id)
        {
            ModelState.AddModelError("", $"Student with {id} already exists.");
            return View();
        }

        private void SendMailWithPassword(Student student)
        {
            var msg = new MailMessage(new MailAddress("admin@university.com", "Admin"), new MailAddress(student.Email, student.FirstName + student.LastName))
            {
                Body = string.Format("Dear {0},{0}{0}Welcome to the Refactoring University.{0}These are your login data.{0}Username: {3}{0}Password: {2}",
                                    student.FirstName,
                                    Environment.NewLine,
                                    student.Password,
                                    student.Email),
                Subject = "New Student Account",
            };
            _mailEngine.Send(msg);
        }

        private ActionResult View()
        {
            return new ActionResult();
        }
    }

    #region Other Classes

    internal class Crypto
    {
        internal static string HashPassword(string password)
        {
            return null;
        }
    }

    internal class PasswordGenerator
    {
        internal static string CreateRandomPassword()
        {
            return null;
        }
    }

    internal class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int ClassId { get; set; }
        public string Telephone { get; set; }
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Password { get; internal set; }
    }

    internal class StudentRepository
    {
        internal static void Add(Student student)
        {
        }

        internal static Student GetById(int id)
        {
            return new Student();
        }
    }

    public class ActionResult
    {
    }

    #endregion
}
