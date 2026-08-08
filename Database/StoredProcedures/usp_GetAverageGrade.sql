-- ============================================================
-- Stored Procedure : usp_GetAverageGrade
-- ============================================================
CREATE PROCEDURE usp_GetAverageGrade
    @AverageGrade DECIMAL(10,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students)
    BEGIN
        THROW 50004, 'No Students Found!', 1;
    END

    SELECT @AverageGrade = AVG(Grade) 
    FROM dbo.Students;
END;