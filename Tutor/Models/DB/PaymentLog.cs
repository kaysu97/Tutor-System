using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.DB
{
    [Table("PaymentLog")]//設定TABLE名稱
    public class PaymentLog
    {



        public PaymentLog()
        {
            //ID = 0;
            //Val = null;
        }

        public PaymentLog(int id, string val)
        {
            // ID = id;
            // Val = val;
        }

        [Key]//設定為Key值
        [Required]//設定為NOT NULL
        [Column(TypeName = "nvarchar")]
        [MaxLength(50)]//指定最大長度
                       //[DatabaseGenerated(DatabaseGeneratedOption.None)]//使其非自動增加
        public string OrderId { get; set; }

        [Column(TypeName = "nvarchar")]//指定欄位型態
        [MaxLength(500)]//指定最大長度
        [Required]
        public string originalData { get; set; }

        [Column(TypeName = "nvarchar")]//指定欄位型態
        [MaxLength(800)]//指定最大長度
        public string SendData { get; set; }

        [Column(TypeName = "datetime")]//指定欄位型態      
        public DateTime SendDateTime { get; set; }

        [Column(TypeName = "nvarchar")]//指定欄位型態
        [MaxLength(800)]//指定最大長度
        public string GetData { get; set; }

        [Column(TypeName = "datetime")]//指定欄位型態      
        public DateTime GetDateTime { get; set; }

        [NotMapped]//不寫入DB
        public string DES { get; set; }





    }
}