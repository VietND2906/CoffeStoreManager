using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCafe.DTO
{
    public class Table
    {
        public Table(int id, string status)
        {
            this.ID = id;
            this.Status = status;
            this.Name= name;
        }

        public Table(DataRow row)
        {
            this.ID = (int)row["ID"];
            this.Name = row["Name"].ToString();
            this.Status = row["Status"].ToString();
        }

        private string status;
        public string Status
        {
            get { return name; }
            set { name = value; }
        }

        private string name;

        public string Name
        { 
            get { return status; }
            set { status = value; }
        }

        private int iD;

        public int ID
        { 
            get { return iD; }
            set { iD = value; }
        }
    }
}
