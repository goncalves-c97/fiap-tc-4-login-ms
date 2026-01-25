using Infra.Data.SqlServer;
using Xunit;

namespace Test.Infra.Data.SqlServer
{
    public class EnsureDatabaseExistsTests
    {
        [Fact]
        public void EnsureDatabaseExists_WhenConnectionStringInvalid_Throws()
        {
            // invalid host makes SqlConnection.Open fail deterministically
            Assert.ThrowsAny<Exception>(() => DatabaseInitializer.EnsureDatabaseExists(
                "Server=invalid-host;Database=master;User Id=sa;Password=bad;TrustServerCertificate=True;Connect Timeout=1;",
                "AuthDb"));
        }

        [Fact]
        public void EnsureDatabaseExists_WhenConnectionStringMalformed_Throws()
        {
            Assert.ThrowsAny<Exception>(() => DatabaseInitializer.EnsureDatabaseExists(
                "not-a-connection-string",
                "AuthDb"));
        }
    }
}
