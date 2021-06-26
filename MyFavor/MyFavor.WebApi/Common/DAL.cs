using MyFavor.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyFavor.WebApi.Common
{
    public class DAL
    {
        private static DAL _instance;
        private DBHelper _defualtDB;
        private HttpClient _gitHubClient;

        public static DAL Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DAL();

                }
                return _instance;
            }
        }

        internal void AddMyFavor(int[] ids)
        {
            foreach (var repoID in ids)
            {
                var insertSql = @"if(not exists( select 1 from [UserFavorRepos] where UserID =@UserID and RepoID = @RepoID))
                    begin
	                    INSERT INTO [dbo].[UserFavorRepos]
			                       ([UserID]
			                       ,[RepoID])
		                     VALUES
			                       (@UserID
			                       ,@RepoID)
                     end";
                var parameters = new SqlParameter[] {
                      new SqlParameter("RepoID",repoID),
                       new SqlParameter("UserID",1),
                };
                _defualtDB.ExecuteNonQuery(insertSql, parameters);
            }
        }

        internal string GetMyFavorDescription()
        {
            var result = string.Empty;
            var getFullNameSql = @"SELECT
      [fullname]
     
  FROM [MyFavorRepos].[dbo].[GitHubRepos] r
  inner join UserFavorRepos u on r.id = u.RepoID
  where u.UserID = @UserID";
            var parameters = new Dictionary<string, object>();
            parameters.Add("UserID", 1);
            var dtFullNames = _defualtDB.ExecSqlStr(getFullNameSql, parameters);
            if (dtFullNames != null && dtFullNames.Rows.Count > 0)
            {
                foreach (DataRow dr in dtFullNames.Rows)
                {
                    var jsonRepos = GetGitHubData("/repos/" + dr["fullname"].ToString()).Result;
                    var tempEntity = new { description = string.Empty };
                    tempEntity = JsonConvert.DeserializeAnonymousType(jsonRepos, tempEntity);
                    if (!string.IsNullOrEmpty(tempEntity.description))
                    {
                        result += tempEntity.description + "\n";
                    }
                }
            }
            return result.Substring(0, result.Length - 2);
        }

        internal void RemoveMyFavor(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                var deleteSql = "delete from UserFavorRepos where UserID =@UserID and RepoID in (" + string.Join(',', ids) + ")";
                var parameters = new SqlParameter[] {

                       new SqlParameter("UserID",1),
                         };
                _defualtDB.ExecuteNonQuery(deleteSql, parameters);
            }
        }

        private DAL()
        {
            _defualtDB = new DBHelper("User ID=ecisuser;Password=sa123456; Initial Catalog=MyFavorRepos;Data Source=DESKTOP-913BTVJ;Connection Timeout=60;");
            _gitHubClient = new HttpClient();
            _gitHubClient.BaseAddress = new Uri("https://api.github.com/");

            _gitHubClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            _gitHubClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");

        }
        public void InitRepos()
        {
            var jsonRepos = GetGitHubData("/users/idcf-boat-house/repos").Result;

            var dtRepos = JsonConvert.DeserializeObject<List<MyFavorModel>>(jsonRepos);
            var delSql = "delete [GitHubRepos]";
            _defualtDB.ExecuteNonQuery(delSql);
            foreach (var item in dtRepos)
            {
                var insertSql = @"INSERT INTO [dbo].[GitHubRepos]
                   ([id]
                  
                   ,[name]
                   ,[fullname]
                   ,[description]
                   ,[htmlurl])
             VALUES
                   (@id,@name,@fullname,@description,@htmlurl)";
                var parameters = new SqlParameter[] {
                      new SqlParameter("id",item.ID),
                      //new SqlParameter("@githubuser",dr["name"].ToString()),
                     
                      new SqlParameter("name",item.Name??string.Empty),
                      new SqlParameter("fullname",item.FullName??string.Empty),
                      new SqlParameter("description",item.Description??string.Empty),
                      new SqlParameter("htmlurl",item.HtmlUrl??string.Empty),
                };


                _defualtDB.ExecuteNonQuery(insertSql, parameters);
            }
        }
        public string GetMyFavor()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("UserID", 1);
            var dtUserFavor = _defualtDB.ExecSqlStr("select * from [UserFavorRepos] where userid=@UserID", parameters);

            List<int> listUserFavorID = new List<int>();
            if (dtUserFavor != null && dtUserFavor.Rows.Count > 0)
            {
                foreach (DataRow dr in dtUserFavor.Rows)
                {
                    listUserFavorID.Add(((int)dr["RepoID"]));
                }
            }
            var dtRepos = _defualtDB.ExecSqlStr("select * from [GitHubRepos]");

            DataSet ds = new DataSet();
            ds.Tables.Add(dtRepos.Clone());
            ds.Tables[0].TableName = "leftTable";
            ds.Tables.Add(dtRepos.Clone());

            ds.Tables[1].TableName = "rightTable";
            foreach (DataRow dr in dtRepos.Rows)
            {

                var id = (int)dr["id"];
                if (!listUserFavorID.Contains(id))
                {
                    ds.Tables["leftTable"].ImportRow(dr);
                }
                else
                {
                    ds.Tables["rightTable"].ImportRow(dr);
                }
            }
            return JsonConvert.SerializeObject(ds);
        }
        private async Task<string> GetGitHubData(string path)
        {
            return await _gitHubClient.GetStringAsync(path);
        }
    }
}
