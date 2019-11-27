SQL SERVER Schema info ref: https://dataedo.com/kb/query/sql-server/list-table-columns-with-their-foreign-keys

select schema_name(tab.schema_id) + '.' + tab.name as ChildTable,
    col.name as ChildKey,
    schema_name(pk_tab.schema_id) + '.' + pk_tab.name as ParentTable,
    pk_col.name as ParentKey
from sys.tables tab
    inner join sys.columns col 
        on col.object_id = tab.object_id
    left outer join sys.foreign_key_columns fk_cols
        on fk_cols.parent_object_id = tab.object_id
        and fk_cols.parent_column_id = col.column_id
    left outer join sys.foreign_keys fk
        on fk.object_id = fk_cols.constraint_object_id
    left outer join sys.tables pk_tab
        on pk_tab.object_id = fk_cols.referenced_object_id
    left outer join sys.columns pk_col
        on pk_col.column_id = fk_cols.referenced_column_id
        and pk_col.object_id = fk_cols.referenced_object_id
where pk_tab.name is not null
order by schema_name(tab.schema_id) + '.' + tab.name,
    col.column_id
	

/*
	To convert result to json you may use free online tool like https://www.csvjson.com/csv2json
*/