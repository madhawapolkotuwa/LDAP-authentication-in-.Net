using System.DirectoryServices.Protocols;

namespace LdapAuthuntication
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Username= ");
                string user = Console.ReadLine();
                Console.Write("Password= ");
                string password = Console.ReadLine();
                if (user != null && password != null)
                {
                    LogResult(user, password);
                }
                else
                {
                    Console.WriteLine("Username & Password can not be null please try again");
                }
            }

            void LogResult(string user, string password)
            {
                LdapConnection conn = new LdapConnection(new LdapDirectoryIdentifier("localhost", 10389))
                {
                    AuthType = AuthType.Basic,
                    Credential = new($"uid={user},dc=example,dc=com", $"{password}")
                };

                conn.SessionOptions.ProtocolVersion = 3;
                
                try
                {
                    conn.Bind();
                    Console.WriteLine($"User {user} Logged In");
                    var request = new SearchRequest(
                    "dc=example,dc=com",
                    $"(&(objectClass=person)(uid={user}))",
                    SearchScope.Subtree,
                    new string[] { "cn", "sn", "employeeType" }
                    );
                    var searchResponse = (SearchResponse)conn.SendRequest(request);

                    var results = searchResponse.Entries.Cast<SearchResultEntry>();

                    if (results.Any())
                    {
                        var resultsEntry = results.First();

                        try
                        {
                            var cn = resultsEntry.Attributes["cn"][0].ToString()!;
                            var sn = resultsEntry.Attributes["sn"][0].ToString()!;
                            var employeeType = resultsEntry.Attributes["employeeType"][0].ToString()!;
                            Console.WriteLine($"cn={cn},\nsn={sn},\nemployeeType={employeeType}");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                    Console.WriteLine("\n\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
