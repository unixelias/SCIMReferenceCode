using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.SCIM.Repository.Utils;

public static class DatabaseHelper
{
    private const char PATH_SEPARATOR = '.';
    private static readonly Assembly CURRENT_ASSEMBLY = Assembly.GetExecutingAssembly();

    public static string LoadSqlStatement(string statementName, string controllerNamespace)
    {
        return LoadResourceFile(string.Format("{0}.Queries", controllerNamespace ?? string.Empty), statementName);
    }

    public static string LoadResourceFile(string resourcePath, string resourceName)
    {
        string sqlStatement = string.Empty;
        var pathBuilder = new StringBuilder();

        pathBuilder.Append(resourcePath);
        pathBuilder.Append(PATH_SEPARATOR);
        pathBuilder.Append(resourceName);

        string sqlResourcePath = pathBuilder.ToString();

        using (Stream stm = CURRENT_ASSEMBLY.GetManifestResourceStream(sqlResourcePath))
        {
            if (stm != null)
            {
                sqlStatement = new StreamReader(stm).ReadToEnd();
            }
        }
        return sqlStatement;
    }
}
