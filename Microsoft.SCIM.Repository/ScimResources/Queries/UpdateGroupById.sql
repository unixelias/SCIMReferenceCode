UPDATE [SCIM].[Group]
   SET group_data = @GroupData
      ,[group_last_update] = GETDATE()
WHERE JSON_VALUE(group_data, '$.id') = @GroupId