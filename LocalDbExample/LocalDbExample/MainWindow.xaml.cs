using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
using Dapper.FastCrud;
using LocalDbExample.Entity;
using LocalDbExample.Entity.Models;

namespace LocalDbExample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Log(string format, params object[] args)
        {
            TextBoxLogArea.Text += string.Format(format, args) + "\n";
        }

        private void LogClear()
        {
            TextBoxLogArea.Text = "";
        }

        /// <summary>
        /// DB情報
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDbInfo_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            using (var conn = ConnectionManager.GetConnection())
            {
                try
                {
                    Log("Connection...");
                    conn.Open();
                    Log("Success!");
                    Log($"Database={conn.Database}");
                    Log($"State={conn.State}");
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// テーブル作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            using (var conn = ConnectionManager.GetConnection())
            {
                Log("Creating Tables...");
                try
                {
                    conn.Execute(@"
create table Departments (
  id int primary key
 ,name nvarchar(32)
 ,updated_at datetime
)");
                    conn.Execute(@"
create table Employees (
  id int primary key
 ,name nvarchar(32)
 ,department_id int
 ,updated_at datetime
)");
                    Log("Success!");
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// テーブル削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrop_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            using (var conn = ConnectionManager.GetConnection())
            {
                Log("Droppping Tables...");
                try
                {
                    conn.Execute(@" drop table Departments ");
                    conn.Execute(@" drop table Employees ");
                    Log("Success!");
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// データ投入ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            var N = 2000;
            try
            {
                using (var conn = ConnectionManager.GetConnection())
                {
                    Log($"Insert records... (N={N})");
                    var dep1 = new Entity.Models.Department() { ID = 5000, NAME = "営業部" };
                    conn.Insert(dep1);
                    var dep2 = new Entity.Models.Department() { ID = 5001, NAME = "開発部" };
                    conn.Insert(dep2);
                    var dep3 = new Entity.Models.Department() { ID = 6000, NAME = "経理部" };
                    conn.Insert(dep3);

                    var emp1 = new Entity.Models.Employee() { Id = 10, Name = "山田太郎", DepartmentId = 5000 };
                    conn.Insert(emp1);
                    var emp2 = new Entity.Models.Employee() { Id = 11, Name = "山田花子", DepartmentId = 5002 };
                    conn.Insert(emp2);
                    var emp3 = new Entity.Models.Employee() { Id = 12, Name = "田中一郎", DepartmentId = 5000 };
                    conn.Insert(emp3);
                    Log("Success!");
                }

                // Microsoft EntityFramework
                using (var db = new Entity.EFModels.Model1())
                {
                    var start = DateTime.Now;
                    for (var i = 10000; i < 10000+N; i++)
                    {
                        db.Employees.Add(new Entity.EFModels.Employee() { id = i, name = "森山太郎", department_id = i/100 });
                        db.SaveChanges();
                    }
                    var finish = DateTime.Now;
                    Log("EF6# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

                // Dapper+FastCrud
                using (var conn = ConnectionManager.GetConnection())
                {
                    var start = DateTime.Now;
                    for (var i = 30000; i < 30000+N; i++)
                    {
                        var emp1 = new Entity.Models.Employee() { Id = i, Name = "広末直美", DepartmentId = i/100 };
                        conn.Insert(emp1);
                    }
                    var finish = DateTime.Now;
                    Log("FC# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        /// <summary>
        /// レコード検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            try
            {
                int i = 0;
                // Dapper+FastCrud
                using (var conn = ConnectionManager.GetConnection())
                {
                    var start = DateTime.Now;
                    foreach (var emp in conn.Find<Entity.Models.Employee>(whereClause: $"department_id<120"))
                    {
                        if (i++ % 100 == 0) Log("{0}", emp.ToString());
                    }
                    var finish = DateTime.Now;
                    Log($"count = {i}");
                    Log("FC# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

                i = 0;
                // Microsoft EntityFramework
                using (var db = new Entity.EFModels.Model1())
                {
                    var start = DateTime.Now;
                    foreach (var emp in from t1 in db.Employees where t1.department_id < 310 select t1)
                    {
                        if (i++ % 100 == 0) Log($"id={emp.id}, name={emp.name}, department_id={emp.department_id}");
                    }
                    var finish = DateTime.Now;
                    Log($"count = {i}");
                    Log("EF6# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        
        /// <summary>
        /// 日本語での検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch2_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            try
            {
                int i = 0;
                // Dapper
                using (var conn = ConnectionManager.GetConnection())
                {
                    var start = DateTime.Now;
                    foreach (var emp in conn.Query<Entity.Models.Employee>("select * from employees where name=@Name", new { Name = "山田太郎" }))
                    {
                        if (i++ % 100 == 0) Log("{0}", emp.ToString());
                    }
                    var finish = DateTime.Now;
                    Log($"count = {i}");
                    Log("Dapper# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

                i = 0;
                // Dapper+FastCrud（プレースホルダマッピングを使用）
                using (var conn = ConnectionManager.GetConnection())
                {
                    var start = DateTime.Now;
                    foreach (var emp in conn.Find<Entity.Models.Employee>(whereClause: $"name=@Name", queryParameters: new { Name = "山田太郎" }))
                    {
                        if (i++ % 100 == 0) Log("{0}", emp.ToString());
                    }
                    var finish = DateTime.Now;
                    Log($"count = {i}");
                    Log("FC1# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

                i = 0;
                // Dapper+FastCrud（SQL文字列だけで検索）
                using (var conn = ConnectionManager.GetConnection())
                {
                    var start = DateTime.Now;
                    foreach (var emp in conn.Find<Entity.Models.Employee>(whereClause:$"name=N'山田太郎'")) //　SQL Serverでは文字列リテラルの前にNが必要
                    {
                        if (i++ % 100 == 0) Log("{0}", emp.ToString());
                    }
                    var finish = DateTime.Now;
                    Log($"count = {i}");
                    Log("FC2# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

                i = 0;
                // Microsoft EntityFramework
                using (var db = new Entity.EFModels.Model1())
                {
                    var start = DateTime.Now;
                    foreach (var emp in from t1 in db.Employees where t1.name == "山田太郎" select t1)
                    {
                        if (i++ % 100 == 0) Log($"id={emp.id}, name={emp.name}, department_id={emp.department_id}");
                    }
                    var finish = DateTime.Now;
                    Log($"count = {i}");
                    Log("EF6# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        /// <summary>
        /// レコード更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            try
            {
                // Dapper+FastCrud
                using (var conn = ConnectionManager.GetConnection())
                {
                    var start = DateTime.Now;
                    foreach (var emp in conn.Find<Entity.Models.Employee>(whereClause: $"department_id<110"))
                    {
                        emp.Name = $"{emp.Name}_{emp.Id}";
                        conn.Update(emp);
                    }
                    var finish = DateTime.Now;
                    Log("FC# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }

                // Microsoft EntityFramework
                using (var db = new Entity.EFModels.Model1())
                {
                    var start = DateTime.Now;
                    foreach (var emp in from t1 in db.Employees where t1.department_id < 310 select t1)
                    {
                        emp.name = $"{emp.name}_{emp.id}";
                    }
                    db.SaveChanges();
                    var finish = DateTime.Now;
                    Log("EF6# start:{0} finish:{1} ({2})", start, finish, finish - start);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }

        }

        /// <summary>
        /// レコード全削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
            using (var conn = ConnectionManager.GetConnection())
            {
                Log("Delete records...");
                try
                {
                    conn.Query($"delete from employees");
                    conn.Query($"delete from departments");
                    Log("Success!");
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }
    }
}
