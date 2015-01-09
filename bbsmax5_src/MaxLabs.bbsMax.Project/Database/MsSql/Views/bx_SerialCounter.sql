CREATE VIEW bx_SerialCounter
AS
SELECT TOP 100 PERCENT s_a.UserID, ISNULL(s_0.S0, 0) AS Unused, ISNULL(s_1.S1, 0) 
      AS Used, ISNULL(s_3.S3, 0) AS Expiress, ISNULL(s_2.S2, 0) AS NoRegister, 
      ISNULL(s_0.S0, 0) + ISNULL(s_1.S1, 0) + ISNULL(s_2.S2, 0) + ISNULL(s_3.S3, 0) 
      AS TotalSerial
FROM (SELECT DISTINCT UserID
        FROM bx_InviteSerials) s_a FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S3
         FROM bx_InviteSerials
         WHERE ExpiresDate <= GETDATE()
         GROUP BY UserID, Status, ExpiresDate
         HAVING Status = 3) s_3 ON s_a.UserID = s_3.UserID FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S2
         FROM bx_InviteSerials
         GROUP BY UserID, Status
         HAVING Status = 2) s_2 ON s_a.UserID = s_2.UserID FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S1
         FROM bx_InviteSerials
         GROUP BY UserID, Status
         HAVING Status = 1) s_1 ON s_a.UserID = s_1.UserID FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S0
         FROM bx_InviteSerials
         GROUP BY UserID, Status
         HAVING Status = 0) s_0 ON s_a.UserID = s_0.UserID
ORDER BY s_a.UserID