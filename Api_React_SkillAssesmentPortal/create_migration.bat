@echo off
echo Creating migration for cascade delete fixes...
cd /d "C:\Users\abdul\Source\Repos\React project\SkillAssesmentPortal\SkillAssesmentPortal"
dotnet ef migrations add FixCascadeDeleteConstraints
echo.
echo Applying migration to database...
dotnet ef database update
echo.
echo Migration completed!
pause