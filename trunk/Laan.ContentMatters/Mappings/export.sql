
    drop table if exists "Comment"

    drop table if exists "News"

    drop table if exists "Post"

    drop table if exists "User"

    drop table if exists "ItemDefinition"

    drop table if exists "Blog"

    create table "Comment" (
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

    create table "News" (
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

    create table "Post" (
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

    create table "User" (
        ID  integer,
       Name TEXT,
       EmailAddress TEXT,
       primary key (ID)
    )

    create table "ItemDefinition" (
        ID  integer,
       MasterPage TEXT,
       TypeName TEXT,
       Created DATETIME,
       Modified DATETIME,
       Description TEXT,
       Title TEXT,
       ItemDefinitionID INTEGER,
       AuthorID INTEGER,
       primary key (ID)
    )

    create table "Blog" (
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
