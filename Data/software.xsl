<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" indent="yes"/>

  <xsl:template match="/">
    <html>
      <head>
        <title>Програмне забезпечення на мережі факультету</title>
        <style>
          body { font-family: Segoe UI, Arial, sans-serif; margin: 20px; }
          table { border-collapse: collapse; width: 100%; }
          th, td { border: 1px solid #999; padding: 4px 8px; vertical-align: top; }
          th { background: #dde4f0; }
          tr:nth-child(even) td { background: #f5f5f5; }
        </style>
      </head>
      <body>
        <h2>Програмне забезпечення на мережі факультету</h2>
        <xsl:if test="Software/@Title">
          <p><b><xsl:value-of select="Software/@Title"/></b></p>
        </xsl:if>
        <p>Кількість програм: <xsl:value-of select="count(Software/Program)"/></p>
        <table>
          <tr>
            <th>№</th>
            <th>Назва</th>
            <th>Вид</th>
            <th>Версія</th>
            <th>Автор</th>
            <th>Факультет</th>
            <th>Кафедра</th>
            <th>Ліцензія</th>
            <th>Анотація</th>
            <th>Умови використання</th>
            <th>Дистрибутив</th>
          </tr>
          <xsl:for-each select="Software/Program">
            <tr>
              <td><xsl:value-of select="position()"/></td>
              <td><b><xsl:value-of select="@Name"/></b></td>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="@Type"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="@Version"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="@Author"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="@Faculty"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="@Department"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="@License"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="Annotation"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="Conditions"/></xsl:call-template>
              <xsl:call-template name="cell"><xsl:with-param name="value" select="Distributive"/></xsl:call-template>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>

  <!-- якщо атрибута немає, пишемо "-" -->
  <xsl:template name="cell">
    <xsl:param name="value"/>
    <td>
      <xsl:choose>
        <xsl:when test="string($value)"><xsl:value-of select="$value"/></xsl:when>
        <xsl:otherwise>-</xsl:otherwise>
      </xsl:choose>
    </td>
  </xsl:template>
</xsl:stylesheet>
