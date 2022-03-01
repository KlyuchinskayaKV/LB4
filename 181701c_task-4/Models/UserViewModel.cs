using System.ComponentModel.DataAnnotations;

namespace _181701c_task_4.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести имя  пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести фамилию пользователя")]
        public string Password { get; set; }
    }
}
