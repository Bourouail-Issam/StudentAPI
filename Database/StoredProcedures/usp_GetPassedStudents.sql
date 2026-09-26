-- ============================================================
-- Stored Procedure : usp_GetPassedStudents
-- ============================================================
CREATE PROCEDURE dbo.usp_GetPassedStudents
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentId, FullName, Age, Grade
    FROM dbo.Students
    WHERE Grade >= 50
    ORDER BY StudentId;
END;
GO