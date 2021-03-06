﻿//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.RegExp
{
	public class TemplateCodeSnippetRegex : Regex
	{
		/* 原正则
\G
\$(?<name>[a-z_]+[a-z_\d]*)
(?>
	(?>
		\<(?>[a-z_]+[a-z_\d]*)(?>\s*,\s*[a-z_]+[a-z_\d]*)*\>
	){0,1}
	\(
		(?<func_param>
			(?>
				"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*"
				|
				'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
				|
				\((?<n>)
				|
				\)(?<-n>)
				|
				(?!\(|\)).
			)*
		)
		(?(n)(?!))
	\)
){0,1}
(?<invoke>
	(?>
		(?>
			\s*\.\s*
			(?>
				(?>[a-z_]+[a-z_\d]*)
				(?>
					(?>
						\<(?>[a-z_]+[a-z_\d]*)(?>\s*,\s*[a-z_]+[a-z_\d]*)*\>
					)
					(?>
						\(
							(?>
								(?>
									"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*"
									|
									'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
									|
									\((?<n>)
									|
									\)(?<-n>)
									|
									(?!\(|\)).
								)*
							)
							(?(n)(?!))
						\)
					)
				){0,1}
				(?>
					\(
						(?>
							(?>
								"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*"
								|
								'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
								|
								\((?<n>)
								|
								\)(?<-n>)
								|
								(?!\(|\)).
							)*
						)
						(?(n)(?!))
					\)
				)*
			)
		)
		|
		(?>
			(?>
				\(
					(?>
						(?>
							"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*"
							|
							'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
							|
							\((?<n>)
							|
							\)(?<-n>)
							|
							(?!\(|\)).
						)*
					)
					(?(n)(?!))
				\)
			)
			|
			(?>
				\[
					(?>
						(?>
							"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*"
							|
							'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
							|
							\[(?<n>)
							|
							\](?<-n>)
							|
							(?!\[|\]).
						)+
					)
					(?(n)(?!))
				\]
			)
		)
	)*
)
		 */

		private const string PATTERN = @"\G
\$(?<name>[a-z_]+[a-z_\d]*)
(?>
	(?>
		\<(?>[a-z_]+[a-z_\d]*)(?>\s*,\s*[a-z_]+[a-z_\d]*)*\>
	){0,1}
	\(
		(?<func_param>
			(?>
				""[^""\\\r\n]*(?:\\.[^""\\\r\n]*)*""
				|
				'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
				|
				\((?<n>)
				|
				\)(?<-n>)
				|
				(?!\(|\)).
			)*
		)
		(?(n)(?!))
	\)
){0,1}
(?<invoke>
	(?>
		(?>
			\s*\.\s*
			(?>
				(?>[a-z_]+[a-z_\d]*)
				(?>
					(?>
						\<(?>[a-z_]+[a-z_\d]*)(?>\s*,\s*[a-z_]+[a-z_\d]*)*\>
					)
					(?>
						\(
							(?>
								(?>
									""[^""\\\r\n]*(?:\\.[^""\\\r\n]*)*""
									|
									'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
									|
									\((?<n>)
									|
									\)(?<-n>)
									|
									(?!\(|\)).
								)*
							)
							(?(n)(?!))
						\)
					)
				){0,1}
				(?>
					\(
						(?>
							(?>
								""[^""\\\r\n]*(?:\\.[^""\\\r\n]*)*""
								|
								'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
								|
								\((?<n>)
								|
								\)(?<-n>)
								|
								(?!\(|\)).
							)*
						)
						(?(n)(?!))
					\)
				)*
			)
		)
		|
		(?>
			(?>
				\(
					(?>
						(?>
							""[^""\\\r\n]*(?:\\.[^""\\\r\n]*)*""
							|
							'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
							|
							\((?<n>)
							|
							\)(?<-n>)
							|
							(?!\(|\)).
						)*
					)
					(?(n)(?!))
				\)
			)
			|
			(?>
				\[
					(?>
						(?>
							""[^""\\\r\n]*(?:\\.[^""\\\r\n]*)*""
							|
							'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*'
							|
							\[(?<n>)
							|
							\](?<-n>)
							|
							(?!\[|\]).
						)+
					)
					(?(n)(?!))
				\]
			)
		)
	)*
)";
		private const RegexOptions OPTIONS = RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Compiled;

		public TemplateCodeSnippetRegex()
			: base(PATTERN, OPTIONS)
		{
		}
	}
}