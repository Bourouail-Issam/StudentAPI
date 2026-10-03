-- ============================================================
-- Test Script: covers every validation branch in usp_AddStudent
-- ============================================================

DECLARE @NewStudentID INT;

PRINT '--- Test 1: Valid data ---';
BEGIN TRY 
    EXEC usp_AddStudent 'Issam Br', 30, 90, @NewStudentID OUTPUT;
    PRINT 'Success! ID = ' + CAST(@NewStudentID AS NVARCHAR(10));
END TRY
BEGIN CATCH
    PRINT 'Error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + ': ' + ERROR_MESSAGE();
END CATCH

-- ############################################

PRINT '--- Test 2: Empty FullName ---';
BEGIN TRY 
    EXEC usp_AddStudent '', 30, 90, @NewStudentID OUTPUT;
    PRINT 'Success! ID = ' + CAST(@NewStudentID AS NVARCHAR(10));
END TRY
BEGIN CATCH
    PRINT 'Error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + ': ' + ERROR_MESSAGE();
END CATCH

-- ############################################

PRINT '--- Test 3: Age too low ---';
BEGIN TRY 
    EXEC usp_AddStudent 'Issam Br', 5, 90, @NewStudentID OUTPUT;
    PRINT 'Success! ID = ' + CAST(@NewStudentID AS NVARCHAR(10));
END TRY
BEGIN CATCH
    PRINT 'Error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + ': ' + ERROR_MESSAGE();
END CATCH

-- ############################################

PRINT '--- Test 4: Age too high ---';
BEGIN TRY 
    EXEC usp_AddStudent 'Issam Br', 200, 90, @NewStudentID OUTPUT;
    PRINT 'Success! ID = ' + CAST(@NewStudentID AS NVARCHAR(10));
END TRY
BEGIN CATCH
    PRINT 'Error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + ': ' + ERROR_MESSAGE();
END CATCH

-- ############################################

PRINT '--- Test 5: Grade too high ---';
BEGIN TRY 
    EXEC usp_AddStudent 'Issam Br', 30, 150, @NewStudentID OUTPUT;
    PRINT 'Success! ID = ' + CAST(@NewStudentID AS NVARCHAR(10));
END TRY
BEGIN CATCH
    PRINT 'Error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + ': ' + ERROR_MESSAGE();
END CATCH