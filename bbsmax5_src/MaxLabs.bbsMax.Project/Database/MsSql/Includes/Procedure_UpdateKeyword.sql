
    SET NOCOUNT ON;

    -- 如果 @Name 是空的，应该在之前的步骤统一去更新版本，不可能到这里不更新 @Name 只更新版本，因此此处只处理 @Name 非空的情况
    IF {TextParam} IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE {Table} SET {Text} = {TextParam}, KeywordVersion = @KeywordVersion WHERE {PrimaryKey} = {PrimaryKeyParam};
        ELSE
            UPDATE {Table} SET {Text} = {TextParam} WHERE {PrimaryKey} = {PrimaryKeyParam};

    END

    IF {TextReverterParam} IS NOT NULL BEGIN

        UPDATE {RevertersTable} SET {TextReverter} = {TextReverterParam} WHERE {PrimaryKey} = {PrimaryKeyParam};
        IF @@ROWCOUNT = 0
            INSERT INTO {RevertersTable} ({PrimaryKey}, {TextReverter}) VALUES ({PrimaryKeyParam}, {TextReverterParam});

    END
