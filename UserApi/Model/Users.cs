﻿using System.ComponentModel.DataAnnotations;

namespace UserApi.Model
{
    public class Users
    {
        [Key]
        public int id_User { get; set; }
        public string Name { get; set; }
        public string Descrioption { get; set; }
    }
}
