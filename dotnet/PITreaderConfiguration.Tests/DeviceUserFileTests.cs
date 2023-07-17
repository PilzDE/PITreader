using System.Text;

namespace Pilz.PITreader.Configuration.Tests
{
    public class DeviceUserFileTests
    {
        [Fact]
        public void ParseTest()
        {
            string inputData = "\"Name\";\"State\";\"Role\";\"Authentication Type\";\"Password Hash\";\"API Token\";\"Remote IP\"\r\n\"admin\";1;500;1;\"D15F1543CA97A1AC9B3DCB059CAB32C8DEAEDCF4B2182BC71665920539A0F00EB68DEF1FABDE2416670A90041261B2B2\";\"64366190A05B0F58ADFB157965AD6B6F\";\"\"\r\n\"UAS\";1;200;2;\"351C69AE7C6C2B9952A96ADD48C0D3EACB9BC2DD478BB0D0582D873C277E47B794DD5306C797AC00337C20802FC311A2\";\"DD5FB30B4D715C0D777EA15559989B77\";\"\"\r\n\"peter\";1;500;1;\"1C2F0D043979C1C23AD6BC392C7902C7A60C4A898FB91D7265E3CA19421FAA1AA82087CCC479A40BEB6519225F36B660\";\"A634286DC7D5C25C3142F996730D51DD\";\"\"\r\n\"CredentialProvider\";1;10;2;\"672721FE5BC1654E0E778CC75A94F7F7912E527EE9309FDC3AA775F457FB3F192202C4D588A9BC65EDFFDAEC62E3EEC3\";\"A10FF77D3A5CC69ABBD6C8E9E5F597D0\";\"192.168.0.25\"\r\n\"ansible\";1;500;2;\"E54CF93E13D2E1BD171F6AF7B80C04C6EFA7B4DE8D2034178F3B17A3B6FC0A78A7E1C5CED348AA3CB07175F7893054F8\";\"45858D5B5AF9D4DF50E3B7A6E9C0478E\";\"\"\r\n";

            var ms = new MemoryStream(Encoding.ASCII.GetBytes(inputData));

            var file = new DeviceUserFile();
            file.Read(ms);

            Assert.True(file.Users.Count == 5);

            var writeStream = new MemoryStream();
            file.Write(writeStream);

            string outData = Encoding.ASCII.GetString(writeStream.ToArray());
            Assert.Equal(inputData, outData);
        }
    }
}