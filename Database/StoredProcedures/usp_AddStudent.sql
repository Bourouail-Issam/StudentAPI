-- ============================================================
-- Stored Procedure : usp_AddStudent
-- ============================================================
CREATE PROCEDURE dbo.usp_AddStudent
    @FullName      NVARCHAR(100),
    @Age           INT,
    @Grade         INT,
    @NewStudentID  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- ==========================================
    -- 1) Validation (matches CHECK Constraints on the table exactly)
    -- ==========================================
    IF (@FullName IS NULL OR LTRIM(RTRIM(@FullName)) = '')
    BEGIN
        THROW 50001, 'FullName cannot be empty.', 1;
        RETURN;
    END

    IF (@Age IS NULL OR @Age < 12 OR @Age > 60)          -- ✅ matches CK_Students_Age
    BEGIN
        THROW 50002, 'Age must be between 12 and 60.', 1;
        RETURN;
    END

    IF (@Grade IS NULL OR @Grade NOT BETWEEN 0 AND 100)  -- ✅ matches CK_Students_Grade
    BEGIN
        THROW 50003, 'Grade must be between 0 and 100.', 1;
        RETURN;
    END

    -- ==========================================
    -- 2) Execution inside a Transaction + Error Handling
    -- ==========================================
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Students (FullName, Age, Grade)
        VALUES (@FullName, @Age, @Grade);

        SET @NewStudentID = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE()) <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO