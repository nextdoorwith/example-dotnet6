using KdfBasicExample;

Console.WriteLine("-----");
{
    var salt = Pbkdf2Example.CreatePasswordSalt();
    var key = Pbkdf2Example.CreateKeyFromPassword("password", salt);
    Console.WriteLine($"{Convert.ToHexString(key)}({key.Length})");
}
