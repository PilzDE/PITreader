using Pilz.PITreader.Client;

var client = new PITreaderClient("192.168.0.16", 443, certificateValidation: CertificateValidators.AcceptAll);
client.ApiToken = "YRqk2wYciMiX4lj2GNWL0Q=="; //  "RYWNW1r51N9Q47em6cBHjg==";

var update = new PITreaderFirmwareManager(client);
var fileName = @"C:\Users\%USERNAME%\Downloads\PITreader_update_2-1-0.zip";

var package = new PITreaderFirmwarePackage(fileName);
await update.PerformUpdateAsync(package, true);

Console.WriteLine("Done");
Console.ReadLine();