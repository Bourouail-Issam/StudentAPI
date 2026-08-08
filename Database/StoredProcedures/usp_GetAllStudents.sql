-- ============================================================
-- Stored Procedure : usp_GetAllStudents
-- ============================================================
CREATE PROCEDURE usp_GetAllStudents
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentId, FullName, Age, Grade
    FROM dbo.Students
    ORDER BY StudentId;
END;