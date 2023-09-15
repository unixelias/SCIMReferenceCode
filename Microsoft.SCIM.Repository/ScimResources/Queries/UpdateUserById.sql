UPDATE [SCIM].[User]
   SET [user_data] = @UserData
      ,[user_last_update] = GETDATE()
WHERE JSON_VALUE(user_data, '$.id') = @UserId