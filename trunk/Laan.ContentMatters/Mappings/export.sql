
    drop table if exists Models_FieldDefinition

    drop table if exists Models_User

    drop table if exists Models_ItemDefinition

    drop table if exists Custom_Comment

    drop table if exists Custom_Post

    drop table if exists Custom_Blog

    create table Models_FieldDefinition (
        ID  integer,
       Name TEXT,
       FieldType INTEGER,
       ReferenceType TEXT,
       ItemDefinitionID INTEGER,
       primary key (ID)
    )
