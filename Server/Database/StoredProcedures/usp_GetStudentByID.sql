-- ============================================================
-- Stored Procedure : usp_GetStudentByID
-- ============================================================
CREATE PROCEDURE dbo.usp_GetStudentByID
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@StudentID <= 0)
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200);
        SET @ErrorMsg = 'Not accepted ID : ' + CAST(@StudentID AS NVARCHAR(10));
        THROW 50006, @ErrorMsg, 1;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @StudentID)
    BEGIN
        DECLARE @ErrorMsg2 NVARCHAR(200);
        SET @ErrorMsg2 = 'Student with ID : ' + CAST(@StudentID AS NVARCHAR(10)) + ' is not found';
        THROW 50007, @ErrorMsg2, 1;
    END;

    SELECT StudentId, FullName, Age, Grade  
    FROM dbo.Students
    WHERE StudentId = @StudentID;

END;