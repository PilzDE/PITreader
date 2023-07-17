using System.Text;

namespace Pilz.PITreader.Configuration.Tests
{
    public class SettingsFileTests
    {
        [Fact]
        public void ParseTest()
        {
            string inputData = @"PITR_CFG
V=01
0x00010800, 01, 0x00
0x00010000, 64, 0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000726564616572746970
0x00010001, 64, 0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0x00010002, 04, 0xc0a8000c
0x00010003, 04, 0xffffff00
0x00010004, 04, 0x00000000
0x00010005, 48, 0x000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0x00010502, 01, 0x01
0x00010503, 04, 0x000001ff
";

            var ms = new MemoryStream(Encoding.ASCII.GetBytes(inputData));

            var file = new SettingsFile();
            file.Read(ms);

            Assert.True(file.Parameter.Count == 9);

            var writeStream = new MemoryStream();
            file.Write(writeStream);

            string outData = Encoding.ASCII.GetString(writeStream.ToArray());
            Assert.Equal(inputData, outData);
        }
    }
}