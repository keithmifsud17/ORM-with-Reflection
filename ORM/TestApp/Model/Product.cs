using ORM.Annotations;
using ORM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Model
{
    [SqlTable("Products")]
    class Product : IBaseModel
    {
        [SqlColumn("product_id", System.Data.SqlDbType.Int, IsKey = true)] public int ID { get; set;}
        [SqlColumn("product_description", System.Data.SqlDbType.VarChar)] public string Description { get; set; }
        [SqlColumn("product_barcode", System.Data.SqlDbType.VarChar)] public string Barcode { get; set; }
    }
}
