-- ============================================================
-- Stored Procedure : usp_GetPassedStudents
-- ============================================================
CREATE PROCEDURE dbo.usp_GetPassedStudents
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students)
    BEGIN
        THROW 50004, 'No Students Found!', 1;
    END

    SELECT StudentId, FullName, Age, Grade
    FROM dbo.Students
    WHERE Grade >= 50
    ORDER BY StudentId;
END;
GO