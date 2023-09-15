DELETE FROM [SCIM].[User]
WHERE JSON_VALUE(user_data, '$.id') = @UserId