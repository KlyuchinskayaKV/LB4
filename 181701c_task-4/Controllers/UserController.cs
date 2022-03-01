using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _181701c_task_4.Models;

namespace _181701c_task_4.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHost;

        public UserController(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        private void Log(string message)
        {
            Console.WriteLine($"Получил валидный запрос на {message}");
        }

        [HttpGet("getUsers")]
        public IActionResult Get()
        {
            Log("получение пользователей");
            try
            {
                List<User> users = new();
                FromFile(users);
                return new JsonResult(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getFile")]
        public IActionResult GetFile()
        {
            Log("получение файла");
            try
            {
                string filePath = Path.Combine(_webHost.ContentRootPath, "test-image.png");
                string fileType = "application/png";
                string fileName = "image.png";
                var path = PhysicalFile(filePath, fileType, fileName);
                if (System.IO.File.Exists(path.FileName) == false){
                    throw new Exception();
                }
                    
                return path;
            }
            catch (Exception)
            {
                return BadRequest("Картинка не найдена");
            }
        }

        [HttpPost("createUser")]
        public IActionResult Post([FromBody] UserViewModel model)
        {
            Log("создания пользователя");
            try
            {
                List<User> users = new();
                FromFile(users);
                int id = 0;

                if (users.Count == 0)
                {
                    users.Add(new User
                    {
                        Id = (id + 1).ToString(),
                        UserName = model.UserName,
                        Password = model.Password
                    });
                }
                else
                {
                    var lastUser = users.Last();
                    id = int.Parse(lastUser.Id);

                    users.Add(new User
                    {
                        Id = (id + 1).ToString(),
                        UserName = model.UserName,
                        Password = model.Password
                    });
                }

                ToFile(users);
                return Ok("Пользователь был успешно создан!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateUser")]
        public IActionResult Put([FromQuery] string id, [FromBody] UserViewModel model)
        {
            Log("модификацию пользователя");
            try
            {

                List<User> users = new();
                FromFile(users);

                if (id == "0")
                    return BadRequest("Необходимо написать Id");
                if (users.Count == 0)
                    return BadRequest("Введены неккоректные данные или база данных пуста");

                foreach (var i in users)
                {
                    if (i.Id == id)
                    {
                        i.UserName = model.UserName;
                        i.Password = model.Password;
                    }
                }

                ToFile(users);
                return Ok("Пользователь успешно обновлен!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("deleteUserById")]
        public IActionResult Delete([FromQuery] string id)
        {
            Log("удаление пользователя");
            try
            {
                List<User> users = new();
                FromFile(users);
                var userUpdate = from user in users
                                 where user.Id == id
                                 select user;

                users.Remove(userUpdate.First());
                ToFile(users);
                return new JsonResult(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private void ToFile(List<User> data)
        {
            using FileStream fstream = new("data.txt", FileMode.Create);
            fstream.Seek(0, SeekOrigin.End);
            StreamWriter sw = new(fstream);
            foreach (User i in data)
            {
                sw.WriteLine(i.Id);
                sw.WriteLine(i.UserName);
                sw.WriteLine(i.Password);
            }
            sw.Close();
        }

        private void FromFile(List<User> data)
        {
            using FileStream fstream = new("data.txt", FileMode.OpenOrCreate);

            StreamReader sw = new(fstream);
            data.Clear();
            while (!sw.EndOfStream)
            {
                User user = new();
                user.Id = sw.ReadLine();
                user.UserName = sw.ReadLine();
                user.Password = sw.ReadLine();
                data.Add(user);
            }
            sw.Close();

        }
    }
}
