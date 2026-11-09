-- ============================================================
-- Stored Procedure : usp_GetAverageGrade
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.usp_GetAverageGrade
    @AverageGrade DECIMAL(10,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @AverageGrade = AVG(CAST(Grade AS DECIMAL(10,2)))
    FROM dbo.Students;

    IF @AverageGrade IS NULL
        THROW 50004, N'No Students Found!', 1;
END;
GO