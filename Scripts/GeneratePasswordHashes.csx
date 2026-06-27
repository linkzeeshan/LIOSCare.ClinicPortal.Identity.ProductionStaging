#r "nuget: Microsoft.AspNetCore.Identity, 8.0.0"

using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<IdentityUser>();
foreach (var (email, password) in new[] {
    ("superadmin@lioscare.local", "SuperAdmin@123"),
    ("clinicadmin@lioscare.local", "Admin@123"),
    ("doctor@lioscare.local", "Doctor@123"),
    ("doctor2@lioscare.local", "Doctor@123")
})
{
    var user = new IdentityUser { Id = Guid.NewGuid().ToString(), UserName = email, Email = email };
    var hash = hasher.HashPassword(user, password);
    Console.WriteLine($"{email} = {hash}");
}
