# setup-db.ps1

function Get-Pbkdf2Hash {
    param([string]$password)
    $saltBytes = New-Object byte[] 16
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $rng.GetBytes($saltBytes)
    $rng.Dispose()

    $pbkdf2 = New-Object System.Security.Cryptography.Rfc2898DeriveBytes($password, $saltBytes, 50000, [System.Security.Cryptography.HashAlgorithmName]::SHA256)
    $hashBytes = $pbkdf2.GetBytes(32)
    $pbkdf2.Dispose()

    return @{
        Hash = [Convert]::ToBase64String($hashBytes)
        Salt = [Convert]::ToBase64String($saltBytes)
    }
}

# 1. Read schema script and execute
Write-Host "Creating schema..."
$schemaScript = Get-Content -Raw -Path "db\01_schema.sql"
# Recreate database to ensure clean schema
$masterConn = New-Object System.Data.SqlClient.SqlConnection("Data Source=localhost;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;")
$masterConn.Open()
$cmd = $masterConn.CreateCommand()
$cmd.CommandText = "IF DB_ID('TaskManagerDb') IS NOT NULL BEGIN ALTER DATABASE TaskManagerDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE TaskManagerDb; END; CREATE DATABASE TaskManagerDb;"
[void]$cmd.ExecuteNonQuery()
$masterConn.Close()

# Run schema
$schemaBatches = $schemaScript -split '(?m)^\s*GO\s*$'
$dbConn = New-Object System.Data.SqlClient.SqlConnection("Data Source=localhost;Initial Catalog=TaskManagerDb;Integrated Security=True;TrustServerCertificate=True;")
$dbConn.Open()
foreach ($batch in $schemaBatches) {
    if (![string]::IsNullOrWhiteSpace($batch)) {
        $cmd = $dbConn.CreateCommand()
        $cmd.CommandText = $batch
        [void]$cmd.ExecuteNonQuery()
    }
}

# 2. Seed Employees with PBKDF2 hashes
Write-Host "Seeding employees..."
$employees = @(
    @{ Name = "Asha Admin"; Email = "admin@taskmanager.local"; Pass = "Admin@12345"; Role = "Admin"; Dept = "Operations" }
    @{ Name = "Mihir Manager"; Email = "manager@taskmanager.local"; Pass = "Manager@12345"; Role = "Manager"; Dept = "Engineering" }
    @{ Name = "Alice Anderson"; Email = "alice@taskmanager.local"; Pass = "Alice@12345"; Role = "Employee"; Dept = "Engineering" }
    @{ Name = "Bob Bhatt"; Email = "bob@taskmanager.local"; Pass = "Bob@12345"; Role = "Employee"; Dept = "Engineering" }
    @{ Name = "Charlie Chen"; Email = "charlie@taskmanager.local"; Pass = "Charlie@12345"; Role = "Employee"; Dept = "QA" }
    @{ Name = "Diana Das"; Email = "diana@taskmanager.local"; Pass = "Diana@12345"; Role = "Employee"; Dept = "Design" }
    @{ Name = "Evan Edwards"; Email = "evan@taskmanager.local"; Pass = "Evan@12345"; Role = "Employee"; Dept = "Engineering" }
    @{ Name = "Fiona Fernandez"; Email = "fiona@taskmanager.local"; Pass = "Fiona@12345"; Role = "Manager"; Dept = "QA" }
    @{ Name = "George Gupta"; Email = "george@taskmanager.local"; Pass = "George@12345"; Role = "Employee"; Dept = "Operations" }
    @{ Name = "Hema Hegde"; Email = "hema@taskmanager.local"; Pass = "Hema@12345"; Role = "Employee"; Dept = "Design" }
)

foreach ($emp in $employees) {
    $creds = Get-Pbkdf2Hash $emp.Pass
    $cmd = $dbConn.CreateCommand()
    $cmd.CommandText = "INSERT INTO dbo.Employees (FullName, Email, PasswordHash, PasswordSalt, Role, Department, IsActive, CreatedAt) VALUES (@Name, @Email, @Hash, @Salt, @Role, @Dept, 1, SYSUTCDATETIME())"
    [void]$cmd.Parameters.AddWithValue("@Name", $emp.Name)
    [void]$cmd.Parameters.AddWithValue("@Email", $emp.Email)
    [void]$cmd.Parameters.AddWithValue("@Hash", $creds.Hash)
    [void]$cmd.Parameters.AddWithValue("@Salt", $creds.Salt)
    [void]$cmd.Parameters.AddWithValue("@Role", $emp.Role)
    [void]$cmd.Parameters.AddWithValue("@Dept", $emp.Dept)
    [void]$cmd.ExecuteNonQuery()
}

# 3. Seed other tables using 02_seed.sql
Write-Host "Seeding database data..."
$seedScript = Get-Content -Raw -Path "db\02_seed.sql"
$cleanSeed = $seedScript -replace '(?mi)^\s*GO\s*$', ' '
$cmd = $dbConn.CreateCommand()
$cmd.CommandText = $cleanSeed
[void]$cmd.ExecuteNonQuery()

# 4. Map IIS APPPOOL\TaskManagerPool permission
Write-Host "Mapping permissions..."
$cmd = $dbConn.CreateCommand()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'IIS APPPOOL\TaskManagerPool') BEGIN CREATE USER [IIS APPPOOL\TaskManagerPool] FOR LOGIN [IIS APPPOOL\TaskManagerPool]; END; ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\TaskManagerPool];"
[void]$cmd.ExecuteNonQuery()

$dbConn.Close()
Write-Host "Database setup complete!"
