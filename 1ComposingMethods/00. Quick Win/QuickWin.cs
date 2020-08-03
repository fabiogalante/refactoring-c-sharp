using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace ComposingMethods.QuickWin
{
    public class QuickWin
    {
        private readonly SmtpClient _mailEngine = new SmtpClient();

        public ActionResult ImportStudents(string csv)
        {
            foreach (string[] cwd in csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(line => line.Split(new[] { '|' })))
            {
                if (!int.TryParse(cwd[0], out var id))
                {
                    ModelState.AddModelError("", $"Student id {id} is not a number.");
                    return View();
                }

                if (StudentRepository.GetById(id) != null)
                {
                    return WorkerAlreadyExistsError(id);
                }

                Student student = CreateWorker(cwd, id);

                var password = PasswordGenerator.CreateRandomPassword();
                student.Password = Crypto.HashPassword(password);

                SendMailWithPassword(new Worker(student.FirstName, student.LastName, student.Email, password));

                StudentRepository.Add(student);
            }


            return View();
        }

        private static Student CreateWorker(string[] d, int id)
        {
            return new Student
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
        }

        private ActionResult WorkerAlreadyExistsError(int id)
        {
            ModelState.AddModelError("", $"Student with {id} already exists.");
            return View();
        }

        private void SendMailWithPassword(Worker worker)
        {
            var msg = new MailMessage(new MailAddress("admin@university.com", "Admin"), new MailAddress(worker.Email, worker.FirstName + worker.LastName))
            {
                Body = string.Format("Dear {0},{0}{0}Welcome to the Refactoring University.{0}These are your login data.{0}Username: {3}{0}Password: {2}",
                                    worker.FirstName,
                                    Environment.NewLine,
                                    worker.Password,
                                    worker.Email),
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
