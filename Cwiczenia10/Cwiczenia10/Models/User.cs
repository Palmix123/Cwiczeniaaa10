using System;
using System.Collections.Generic;

namespace Cwiczenia10.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;
}
