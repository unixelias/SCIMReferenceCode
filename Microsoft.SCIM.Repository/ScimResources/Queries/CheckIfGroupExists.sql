SELECT
    CASE WHEN EXISTS 
    (
        SELECT g.group_data_id
		FROM [SCIM].[Group] [g]
        WHERE JSON_VALUE(group_data, '$.id') = @GroupId OR JSON_VALUE(group_data, '$.displayName') = @DisplayName
    )
    THEN 'TRUE'
    ELSE 'FALSE'
END