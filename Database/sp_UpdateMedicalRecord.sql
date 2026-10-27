-- Lets a doctor edit the medical record of an appointment that is already Completed.
-- sp_CompleteAppointment only accepts Scheduled/Confirmed appointments, so it
-- cannot be used once the appointment has been completed.
--
-- Returns:
--   1  record saved
--  -1  appointment not found, not Completed, or not owned by this doctor

CREATE OR ALTER PROCEDURE sp_UpdateMedicalRecord
    @AppointmentId INT,
    @DoctorUserId INT,
    @Diagnosis NVARCHAR(500),
    @DoctorNotes NVARCHAR(MAX),
    @Medicines NVARCHAR(MAX),
    @Procedures NVARCHAR(MAX),
    @Advice NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Appointments a
        INNER JOIN DoctorSlots ds
            ON a.SlotId = ds.SlotId
        INNER JOIN DoctorSchedules sch
            ON ds.ScheduleId = sch.ScheduleId
        INNER JOIN Doctors d
            ON sch.DoctorId = d.DoctorId
        WHERE a.AppointmentId = @AppointmentId
          AND d.UserId = @DoctorUserId
          AND a.Status = 'Completed'
    )
    BEGIN
        RETURN -1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM MedicalRecords
        WHERE AppointmentId = @AppointmentId
    )
    BEGIN
        UPDATE MedicalRecords
        SET Diagnosis = @Diagnosis,
            DoctorNotes = @DoctorNotes,
            Medicines = @Medicines,
            Procedures = @Procedures,
            Advice = @Advice
        WHERE AppointmentId = @AppointmentId;
    END
    ELSE
    BEGIN
        INSERT INTO MedicalRecords
        (
            AppointmentId,
            Diagnosis,
            DoctorNotes,
            Medicines,
            Procedures,
            Advice
        )
        VALUES
        (
            @AppointmentId,
            @Diagnosis,
            @DoctorNotes,
            @Medicines,
            @Procedures,
            @Advice
        );
    END;

    RETURN 1;
END
