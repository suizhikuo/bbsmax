    SET NOCOUNT ON;

    -- 如果 {Text1Param} 是空的，应该在之前的步骤统一去更新版本，不可能到这里不更新 {Text1Param} 只更新版本，因此此处只处理 {Text1Param} 非空的情况
    IF {Text1Param} IS NOT NULL OR {Text2Param} IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL BEGIN

            IF {Text1Param} IS NOT NULL AND {Text2Param} IS NOT NULL
                UPDATE {Table} SET {Text1} = {Text1Param}, {Text2} = {Text2Param}, KeywordVersion = @KeywordVersion WHERE {PrimaryKey} = {PrimaryKeyParam};
            ELSE IF {Text1Param} IS NOT NULL
                UPDATE {Table} SET {Text1} = {Text1Param}, KeywordVersion = @KeywordVersion WHERE {PrimaryKey} = {PrimaryKeyParam};
            ELSE
                UPDATE {Table} SET {Text2} = {Text2Param}, KeywordVersion = @KeywordVersion WHERE {PrimaryKey} = {PrimaryKeyParam};

        END
        ELSE BEGIN

           IF {Text1Param} IS NOT NULL AND {Text2Param} IS NOT NULL
                UPDATE {Table} SET {Text1} = {Text1Param}, {Text2} = {Text2Param} WHERE {PrimaryKey} = {PrimaryKeyParam};
            ELSE IF {Text1Param} IS NOT NULL
                UPDATE {Table} SET {Text1} = {Text1Param} WHERE {PrimaryKey} = {PrimaryKeyParam};
            ELSE
                UPDATE {Table} SET {Text2} = {Text2Param} WHERE {PrimaryKey} = {PrimaryKeyParam};

        END

    END

    IF ({Text1ReverterParam} IS NOT NULL AND {Text2ReverterParam} IS NOT NULL) BEGIN

        UPDATE {RevertersTable} SET {Text1Reverter} = {Text1ReverterParam}, {Text2Reverter} = {Text2ReverterParam} WHERE {PrimaryKey} = {PrimaryKeyParam};
        IF @@ROWCOUNT = 0
            INSERT INTO {RevertersTable} ({PrimaryKey}, {Text1Reverter}, {Text2Reverter}) VALUES ({PrimaryKeyParam}, {Text1ReverterParam}, {Text2ReverterParam});

    END
    ELSE IF ({Text1ReverterParam} IS NOT NULL) BEGIN

        UPDATE {RevertersTable} SET {Text1Reverter} = {Text1ReverterParam} WHERE {PrimaryKey} = {PrimaryKeyParam};
        IF @@ROWCOUNT = 0
            INSERT INTO {RevertersTable} ({PrimaryKey}, {Text1Reverter}, {Text2Reverter}) VALUES ({PrimaryKeyParam}, {Text1ReverterParam}, N'');

    END
    ELSE IF ({Text2ReverterParam} IS NOT NULL) BEGIN

        UPDATE {RevertersTable} SET {Text2Reverter} = {Text2ReverterParam} WHERE {PrimaryKey} = {PrimaryKeyParam};
        IF @@ROWCOUNT = 0
            INSERT INTO {RevertersTable} ({PrimaryKey}, {Text1Reverter}, {Text2Reverter}) VALUES ({PrimaryKeyParam}, N'', {Text2ReverterParam});

    END