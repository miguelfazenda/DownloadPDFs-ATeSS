import re
import html
import json
from enum import Enum
from datetime import datetime

class TipoReciboVerde(Enum):
    Pagamento = 0
    Adiantamento = 1
    AdiantamentoPagamento = 2
    Fatura = 3
    Recibo = 4

class TipoReciboVerdePrestOUAdquir(Enum):
    Prestador = 0
    Adquirente = 1

class RecibosVerdesValores:
    def __init__(self, valor_base=0, valor_iva_continente=0, imposto_selo=0, irs_sem_retencao=0, importancia_recebida=0):
        self.valor_base = valor_base
        self.valor_iva_continente = valor_iva_continente
        self.imposto_selo = imposto_selo
        self.irs_sem_retencao = irs_sem_retencao
        self.importancia_recebida = importancia_recebida

    def __add__(self, other):
        return RecibosVerdesValores(
            self.valor_base + other.valor_base,
            self.valor_iva_continente + other.valor_iva_continente,
            self.imposto_selo + other.imposto_selo,
            self.irs_sem_retencao + other.irs_sem_retencao,
            self.importancia_recebida + other.importancia_recebida
        )

class ReciboVerdeValores:
    def __init__(self, valor_base=0, valor_iva_continente=0, imposto_selo=0, irs_sem_retencao=0, importancia_recebida=0):
        self.valor_base = valor_base
        self.valor_iva_continente = valor_iva_continente
        self.imposto_selo = imposto_selo
        self.irs_sem_retencao = irs_sem_retencao
        self.importancia_recebida = importancia_recebida

class ReciboVerde:
    def __init__(self):
        self.tipo_doc = None
        self.num_doc = None
        self.nif_prestador_servicos = None
        self.nif_adquirente = None
        self.descricao = None
        self.nome_adquirente = None
        self.nome_prestador = None
        self.pais_adquirente = None
        self.data_emissao = None
        self.data_prestacao_servico = None
        self.details_url = None
        self.anulado = False
        self.valores = None
        self.tipo_recibo_verde = None
        self.tipo = None  # Prestador ou Adquirente
    def __str__(self):
        return f"{self.num_doc} {self.tipo_doc} {self.nome_prestador} -> {self.nome_adquirente} ({self.data_emissao})"
class ScraperRecibosVerdes:
    regex_recibo_verde_data_emissao = re.compile(r"\d\d\d\d-\d\d-\d\d")

    @staticmethod
    def obter_dados_recibo_verde(details_url, tipo, driver, ano):
        driver.get(details_url)
        recibo_verde = ReciboVerde()
        recibo_verde.details_url = details_url
        recibo_verde.tipo = tipo
        dados = driver.find_element('xpath', "//*[@id='main-content']/div/div/detalhe-fatura-recibo-app-v2").get_attribute("info-documento")
        json_str = html.unescape(dados)
        dto = json.loads(json_str)
        recibo_verde.tipo_doc = dto.get('tipoDocumento')
        recibo_verde.num_doc = str(dto.get('numDocumento'))
        recibo_verde.nif_prestador_servicos = str(dto.get('nifPrestadorServicos'))
        recibo_verde.nif_adquirente = dto.get('nifAdquirente')
        recibo_verde.nome_prestador = dto.get('nomePrestador')
        recibo_verde.nome_adquirente = dto.get('nomeAdquirente')
        linhas_mapeadas = dto.get('linhasMapeadas')
        if linhas_mapeadas and len(linhas_mapeadas) > 0:
            recibo_verde.descricao = linhas_mapeadas[0].get('descricao')
        else:
            recibo_verde.descricao = ""
        recibo_verde.pais_adquirente = dto.get('paisDescr')
        recibo_verde.data_emissao = datetime.strptime(dto.get('dataEmissao'), "%Y-%m-%d")
        recibo_verde.data_prestacao_servico = datetime.strptime(dto.get('dataPrestacaoServico'), "%Y-%m-%d")
        recibo_verde.details_url = dto.get('urlNotasCreditoDoDoc')
        recibo_verde.anulado = dto.get('situacaoCod') != "E"
        valores = RecibosVerdesValores(
            valor_base=dto.get('valorBase', 0),
            valor_iva_continente=dto.get('valorIVA', 0),
            imposto_selo=dto.get('valorImpostoSelo', 0),
            irs_sem_retencao=dto.get('valorIRS', 0),
            importancia_recebida=dto.get('importanciaRecebida', 0)
        )
        recibo_verde.valores = valores
        tipo_documento_codigo = dto.get('tipoDocumentoCodigo')
        if tipo_documento_codigo == "FR":
            recibo_verde.tipo_recibo_verde = TipoReciboVerde.Pagamento
        elif tipo_documento_codigo == "F":
            recibo_verde.tipo_recibo_verde = TipoReciboVerde.Fatura
        elif tipo_documento_codigo == "R":
            recibo_verde.tipo_recibo_verde = TipoReciboVerde.Recibo
        else:
            recibo_verde.tipo_recibo_verde = TipoReciboVerde.Pagamento
        recibo_verde.tipo = TipoReciboVerdePrestOUAdquir.Prestador
        return recibo_verde

    @staticmethod
    def get_recibo_tipo_string(tipo_recibo_verde):
        if tipo_recibo_verde == TipoReciboVerde.Pagamento:
            return "Pagamento"
        elif tipo_recibo_verde == TipoReciboVerde.Adiantamento:
            return "Adiantamento"
        elif tipo_recibo_verde == TipoReciboVerde.AdiantamentoPagamento:
            return "Adiantamento para pagamento"
        return "-"
