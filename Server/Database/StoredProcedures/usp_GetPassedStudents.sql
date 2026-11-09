-- ============================================================
-- Stored Procedure : usp_GetPassedStudents
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.usp_GetPassedStudents
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentId, UserId, FullName, Age, Grade
    FROM dbo.Students
    WHERE Grade >= 50
    ORDER BY StudentId;
END;
GO