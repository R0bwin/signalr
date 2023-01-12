using System.ComponentModel.DataAnnotations;

namespace Socket
{
    public class User
    {
        [Key]
        public string Connection { get; set; }
        public string UserName { get; set; }
        public virtual string Room { get; set; }
    }
}
