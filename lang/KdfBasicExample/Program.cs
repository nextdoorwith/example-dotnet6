using KdfBasicExample;

Console.WriteLine("-----");
{
    var salt = Pbkdf2Example.CreatePasswordSalt();
    var key = Pbkdf2Example.CreateKeyFromPassword("password", salt);

    Console.WriteLine($"IV : {Convert.ToHexString(salt)}({salt.Length})");
    Console.WriteLine($"Key: {Convert.ToHexString(key)}({key.Length})");
}
