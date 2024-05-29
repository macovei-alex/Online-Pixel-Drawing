// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

while (true)
{
	string message = Console.ReadLine();
	if (message == null)
		continue;

	string hash = ComputeSHA256Hash(message);
	Console.WriteLine(hash);
}

static string ComputeSHA256Hash(string input)
{
	using SHA256 sha256Hash = SHA256.Create();
	byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
	StringBuilder builder = new();

	for (int i = 0; i < bytes.Length; i++)
	{
		builder.Append(bytes[i].ToString("x2"));
	}

	return builder.ToString();
}