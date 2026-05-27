using System;
using System.DirectoryServices;
using System.Text.RegularExpressions;

namespace Referralcode.Services
{
    public class AuthLib
    {
        private string g_strAdPath;
        private string g_strDomainName;
        private string g_strUsrId;
        private string g_strUsrPwd;

        public string getLdapAuthRes()
        {
            DirectoryEntry dirEntry = null;
            DirectorySearcher dirSearcher = null;
            try
            {
#pragma warning disable CA1416 // Validate platform compatibility
                dirEntry = new DirectoryEntry(
                    this.g_strAdPath,
                    this.g_strDomainName + "\\" + this.g_strUsrId,
                    this.g_strUsrPwd,
                    AuthenticationTypes.Secure);

                dirSearcher = new DirectorySearcher(dirEntry);
                // 移除非英數字元以防止 LDAP Injection
                var safeUsrId = Regex.Replace(this.g_strUsrId, @"[^\w]", string.Empty);
                dirSearcher.Filter = "(SAMAccountName=" + safeUsrId + ")";

                SearchResult searchResult = dirSearcher.FindOne();
                if (searchResult == null)
                {
                    return "AD 驗證失敗";
                }
                else
                {
                    this.g_strAdPath = searchResult.Path;
                    return "Success";
                }
#pragma warning restore CA1416
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
#pragma warning disable CA1416
                dirSearcher?.Dispose();
                dirEntry?.Dispose();
#pragma warning restore CA1416
            }
        }

        public void setAdPath(string strAdPath)
        {
            this.g_strAdPath = strAdPath;
        }

        public void setDomainName(string strDomainName)
        {
            this.g_strDomainName = strDomainName;
        }

        public void setUsrId(string strUsrId)
        {
            this.g_strUsrId = strUsrId;
        }

        public void setUsrPwd(string strUsrPwd)
        {
            this.g_strUsrPwd = strUsrPwd;
        }
    }
}
