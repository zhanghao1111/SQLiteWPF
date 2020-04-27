using SQLiteWPF.Model;
using SQLiteWPF.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SQLiteWPF.Dao
{
    /// <summary>
    /// 公司数据库操作
    /// </summary>
    class CompanySQLiteDao
    {
        /// <summary>
        /// 查询所有公司的注册信息
        /// </summary>
        /// <returns>所有注册公司的记录信息</returns>
        public IList<CompanyModel> Qurey(int Type)
        {
            string sql = "SELECT in1.*, tp.Type FROM info in1, type tp WHERE in1.Type = tp.Type AND in1.Type = @Type ORDER BY in1.Name";

            SQLiteParameter[] parameters = 
                                 {
                                     new SQLiteParameter("@Type", DbType.Int32)
                                 };
            parameters[0].Value = Type;
            return SQLiteHelper.GetList<CompanyModel>(sql, parameters);
        }
        /// <summary>
        /// 根据公司名查询此公司详细信息
        /// </summary>
        /// <param name="Name">公司名</param>
        /// <returns>公司详细信息</returns>
        public IList<CompanyModel> Qurey(string Name)
        {
            string sql = "SELECT * FROM info WHERE Name = @Name";
            SQLiteParameter[] parameters =
            {
                 new SQLiteParameter("@Name", Name),
            };

            return SQLiteHelper.GetList<CompanyModel>(sql, parameters);
        }
        /// <summary>
        /// 插入公司信息
        /// </summary>
        /// <param name="companyModel">公司信息</param>
        /// <returns></returns>
        public int Insert(CompanyModel companyModel)
        {
            string sql = "INSERT INTO info(Name,Address,Telephone,LegalPerson,RegistrationDate) " +
                         "VALUES(@Name,@Address,@Telephone,@LegalPerson,@RegistrationDate)";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@Name",DbType.String),
                new SQLiteParameter("@Address",DbType.String),
                new SQLiteParameter("@Telephone",DbType.String),
                new SQLiteParameter("@LegalPerson",DbType.String),
                new SQLiteParameter("@RegistrationDate",DbType.String),
            };
            //参数赋值
            parameters[0].Value = companyModel.Name;
            parameters[1].Value = companyModel.Address;
            parameters[2].Value = companyModel.Telephone;
            parameters[3].Value = companyModel.LegalPerson;
            parameters[4].Value = companyModel.RegistrationDate;

            return SQLiteHelper.ExecuteSql(sql, parameters);
        }
        /// <summary>
        /// 更新一条公司的详细信息
        /// </summary>
        /// <param name="companyModel">公司信息</param>
        /// <returns></returns>
        public int Update(CompanyModel companyModel)
        {
            //Name = @Name,Address = @Address,Telephone = @Telephone,LegalPerson = @LegalPerson,
            string sql = "UPDATE info SET Address = @Address,RegistrationDate = @RegistrationDate WHERE Name = @Name";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@Name",DbType.String),
                new SQLiteParameter("@Address",DbType.String),
                //new SQLiteParameter("@Telephone",DbType.String),
                //new SQLiteParameter("@LegalPerson",DbType.String),
                new SQLiteParameter("@RegistrationDate",DbType.String),
            };
            //参数赋值
            parameters[0].Value = companyModel.Name;
            parameters[1].Value = companyModel.Address;
            //parameters[2].Value = companyModel.Telephone;
            //parameters[3].Value = companyModel.LegalPerson;
            parameters[2].Value = companyModel.RegistrationDate;

            return SQLiteHelper.ExecuteSql(sql,parameters);
        }
        /// <summary>
        /// 根据公司名删除一条公司信息
        /// </summary>
        /// <param name="Name">公司名</param>
        /// <returns></returns>
        public int Delete(string Name)
        {
            string sql = "DELETE FROM info WHERE Name = @Name";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@Name",DbType.String),
            };
            //参数赋值
            parameters[0].Value = Name;
            return SQLiteHelper.ExecuteSql(sql,parameters);
        }
    }
}