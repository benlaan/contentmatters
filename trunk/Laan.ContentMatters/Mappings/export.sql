
    drop table if exists Custom_Comment

    drop table if exists Custom_News

    drop table if exists Models_FieldDefinition

    drop table if exists Custom_Post

    drop table if exists Models_User

    drop table if exists Models_ItemDefinition

    drop table if exists Custom_Blog

    create table Custom_Comment (
        ID  integer,
       Body TEXT,
       TypeName TEXT,
       Created DATETIME,
       Modified DATETIME,
       Description TEXT,
       Title TEXT,
       ItemDefinitionID INTEGER,
       AuthorID INTEGER,
       PostID INTEGER,
       primary key (ID)
    )

    create table Custom_News (
        ID  integer,
       TypeName TEXT,
       Created DATETIME,
       Modified DATETIME,
       Description TEXT,
       Title TEXT,
       ItemDefinitionID INTEGER,
       AuthorID INTEGER,
       primary key (ID)
    )

    create table Models_FieldDefinition (
        ID  integer,
       Name TEXT,
       FieldType INTEGER,
       ItemDefinitionID INTEGER,
       primary key (ID)
    )

    create table Custom_Post (
        ID  integer,
       Body TEXT,
       TypeName TEXT,
       Created DATETIME,
       Modified DATETIME,
       Description TEXT,
       Title TEXT,
       ItemDefinitionID INTEGER,
       AuthorID INTEGER,
       BlogID INTEGER,
       primary key (ID)
    )

    create table Models_User (
        ID  integer,
       Name TEXT,
       EmailAddress TEXT,
       primary key (ID)
    )

    create table Models_ItemDefinition (
        ID  integer,
       MasterPage TEXT,
       Namespace TEXT,
       Description TEXT,
       Name TEXT,
       primary key (ID)
    )

    create table Custom_Blog (
        ID  integer,
       TypeName TEXT,
       Created DATETIME,
       Modified DATETIME,
       Description TEXT,
       Title TEXT,
       ItemDefinitionID INTEGER,
       AuthorID INTEGER,
       primary key (ID)
    )
