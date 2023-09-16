DELETE FROM [SCIM].[Group]
WHERE JSON_VALUE(group_data, '$.id') = @GroupId