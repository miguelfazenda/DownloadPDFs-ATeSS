# Hardcoded export definitions (from definicoes.json)
definicoes_exportacao_prestador = {
    "defExportFaturaRecibo": {
        "defExportTipoPagamento": {
            "TipoDocumento": "Fatura-Recibo",
            "TipoReciboVerde": "Pagamento",
            "diario": "58",
            "tipoDoc": "518",
            "contaValBase": "721113",
            "contaValBaseIsento": "72113",
            "contaIVA": "2433113",
            "contaSelo": None,
            "contaIRS": "2412",
            "contaValRecebida": "111"
        },
        "defExportTipoAdiantamento": {
            "TipoDocumento": "Fatura-Recibo",
            "TipoReciboVerde": "Adiantamento",
            "diario": "58",
            "tipoDoc": "518",
            "contaValBase": None,
            "contaValBaseIsento": "211110000",
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": "111"
        },
        "defExportTipoAdiantamentoPagamento": {
            "TipoDocumento": "Fatura-Recibo",
            "TipoReciboVerde": "Adiant. para despesas",
            "diario": "58",
            "tipoDoc": "518",
            "contaValBase": None,
            "contaValBaseIsento": "211110000",
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": "111"
        },
        "defExportTipoFaturaOuRecibo": None
    },
    "defExportFatura": {
        "defExportTipoPagamento": None,
        "defExportTipoAdiantamento": None,
        "defExportTipoAdiantamentoPagamento": None,
        "defExportTipoFaturaOuRecibo": {
            "TipoDocumento": "Fatura",
            "TipoReciboVerde": "Fatura",
            "diario": "58",
            "tipoDoc": "511",
            "contaValBase": "721113",
            "contaValBaseIsento": "72113",
            "contaIVA": "2433113",
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": "211110000"
        }
    },
    "defExportRecibo": {
        "defExportTipoPagamento": None,
        "defExportTipoAdiantamento": None,
        "defExportTipoAdiantamentoPagamento": None,
        "defExportTipoFaturaOuRecibo": {
            "TipoDocumento": "Recibo",
            "TipoReciboVerde": "Recibo",
            "diario": "58",
            "tipoDoc": "212",
            "contaValBase": None,
            "contaValBaseIsento": "211110000",
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": "2412",
            "contaValRecebida": "111"
        }
    }
}

definicoes_exportacao_adquirente = {
    "defExportFaturaRecibo": {
        "defExportTipoPagamento": {
            "TipoDocumento": "Fatura-Recibo",
            "TipoReciboVerde": "Pagamento",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        },
        "defExportTipoAdiantamento": {
            "TipoDocumento": "Fatura-Recibo",
            "TipoReciboVerde": "Adiantamento",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        },
        "defExportTipoAdiantamentoPagamento": {
            "TipoDocumento": "Fatura-Recibo",
            "TipoReciboVerde": "Adiant. para despesas",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        },
        "defExportTipoFaturaOuRecibo": None
    },
    "defExportFatura": {
        "defExportTipoPagamento": {
            "TipoDocumento": "Fatura",
            "TipoReciboVerde": "Fatura",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        },
        "defExportTipoAdiantamento": None,
        "defExportTipoAdiantamentoPagamento": None,
        "defExportTipoFaturaOuRecibo": {
            "TipoDocumento": "Fatura",
            "TipoReciboVerde": "Fatura",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        }
    },
    "defExportRecibo": {
        "defExportTipoPagamento": {
            "TipoDocumento": "Recibo",
            "TipoReciboVerde": "Recibo",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        },
        "defExportTipoAdiantamento": None,
        "defExportTipoAdiantamentoPagamento": None,
        "defExportTipoFaturaOuRecibo": {
            "TipoDocumento": "Recibo",
            "TipoReciboVerde": "Recibo",
            "diario": None,
            "tipoDoc": None,
            "contaValBase": None,
            "contaValBaseIsento": None,
            "contaIVA": None,
            "contaSelo": None,
            "contaIRS": None,
            "contaValRecebida": None
        }
    }
}

def get_export_defs(tipo, tipo_doc, tipo_recibo_verde):
    """
    tipo: 'Prestador' or 'Adquirente' (str or Enum.name)
    tipo_doc: 'Fatura-Recibo', 'Fatura', 'Recibo'
    tipo_recibo_verde: 'Pagamento', 'Adiantamento', 'Adiant. para despesas', 'Fatura', 'Recibo'
    Returns: dict with export definitions or None
    """
    if hasattr(tipo, 'name'):
        tipo = tipo.name
    defs = definicoes_exportacao_prestador if tipo == 'Prestador' else definicoes_exportacao_adquirente
    if tipo_doc == 'Fatura-Recibo':
        doc_defs = defs['defExportFaturaRecibo']
    elif tipo_doc == 'Fatura':
        doc_defs = defs['defExportFatura']
    elif tipo_doc == 'Recibo':
        doc_defs = defs['defExportRecibo']
    else:
        return None
    
    definicao = None
    # Map tipo_recibo_verde to the correct sub-definition
    if tipo_recibo_verde == 'Pagamento':
        definicao =  doc_defs.get('defExportTipoPagamento')
    elif tipo_recibo_verde == 'Adiantamento':
        definicao = doc_defs.get('defExportTipoAdiantamento')
    elif tipo_recibo_verde == 'Adiant. para despesas':
        definicao = doc_defs.get('defExportTipoAdiantamentoPagamento')
    elif tipo_recibo_verde in ('Fatura', 'Recibo'):
        definicao = doc_defs.get('defExportTipoFaturaOuRecibo')
    
    return definicao