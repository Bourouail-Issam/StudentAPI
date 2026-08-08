-- ============================================================
-- Stored Procedure : usp_UpdateStudent
-- ============================================================
CREATE PROCEDURE dbo.usp_UpdateStudent
    @UpdateStudentID INT,
    @FullName        NVARCHAR(100),
    @Age             INT,
    @Grade           INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@UpdateStudentID IS NULL)
    BEGIN
        THROW 50008, 'StudentID cannot be Null', 1;
    END;

    IF (@UpdateStudentID <= 0)
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200);
        SET @ErrorMsg = 'Not accepted ID : ' + CAST(@UpdateStudentID AS NVARCHAR(10));
        THROW 50006, @ErrorMsg, 1;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @UpdateStudentID)
    BEGIN
        DECLARE @ErrorMsg2 NVARCHAR(200);
        SET @ErrorMsg2 = 'Student with ID : ' + CAST(@UpdateStudentID AS NVARCHAR(10)) + ' is not found';
        THROW 50007, @ErrorMsg2, 1;
    END;

    -- ==========================================
    -- 1) Validation (matches CHECK Constraints on the table exactly)
    -- ==========================================
    IF (@FullName IS NULL OR LTRIM(RTRIM(@FullName)) = '')
    BEGIN
        THROW 50001, 'FullName cannot be empty.', 1;
    END;

    IF (@Age IS NULL OR @Age < 12 OR @Age > 60)          -- ✅ matches CK_Students_Age
    BEGIN
        THROW 50002, 'Age must be between 12 and 60.', 1;
    END;

    IF (@Grade IS NULL OR @Grade NOT BETWEEN 0 AND 100)  -- ✅ matches CK_Students_Grade
    BEGIN
        THROW 50003, 'Grade must be between 0 and 100.', 1;
    END;

    -- ==========================================
    -- 2) Execution inside a Transaction + Error Handling
    -- ==========================================
    BEGIN TRY
        BEGIN TRANSACTION
            UPDATE dbo.Students
            SET FullName    = @FullName,
                Age         = @Age,
                Grade       = @Grade
            WHERE StudentId = @UpdateStudentID;
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF (XACT_STATE()) <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;