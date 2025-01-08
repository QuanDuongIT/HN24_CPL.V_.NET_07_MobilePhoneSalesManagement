﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.DAL.Models.AuthenticationModels
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateExpire { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}