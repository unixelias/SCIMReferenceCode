SELECT
    CASE WHEN EXISTS 
    (
        SELECT u.user_data_id
		FROM [SCIM].[User] [u]
        WHERE JSON_VALUE(user_data, '$.id') = @UserId OR JSON_VALUE(user_data, '$.userName') = @UserName
    )
    THEN 'TRUE'
    ELSE 'FALSE'
END