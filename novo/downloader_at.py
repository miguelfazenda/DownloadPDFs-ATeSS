from datetime import datetime
import html
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import Select
import time
from selenium.webdriver.common.by import By
import json
import dl
from scraper_recibos_verdes import (
    ReciboVerde,
    ReciboVerdeValores,
    TipoReciboVerdePrestOUAdquir,
)
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class DownloaderAT:
    @staticmethod
    def download_modelo22(
        driver,
        ano,
        expect_download,
        wait_for_download_finish,
        gen_novo_nome_ficheiro,
        declaracao,
        estrutura_nomes_ficheiros,
    ):
        url = (
            f"https://irc.portaldasfinancas.gov.pt/mod22/obter-comprovativo#!?ano={ano}"
        )
        driver.get(url)
        driver.get(url)  # C# does this twice
        expect_download()
        if dl.Downloader.click_button_wait_for_it_to_appear(
            driver, (By.XPATH, DownloaderAT.XPATH_MODELO22_BOTAO_OBTER)
        ):
            wait_for_download_finish(
                gen_novo_nome_ficheiro(estrutura_nomes_ficheiros.AT_Modelo22),
                declaracao.AT_Modelo22,
                0,
            )
        else:
            dl.Downloader.log("Modelo 22", "Sem resultados")

    XPATH_MODELO22_BOTAO_OBTER = "/html/body/div/main/div/div[2]/div/section/div/consultar-comprovativo-app/div[3]/consultar-comprovativo-tabela/div/div[2]/div/div/table/tbody/tr/td[5]/button"

    @staticmethod
    def get_recibos_verdes_lista(
        driver, ano, mes, tipo, empresa_autenticada
    ) -> list[ReciboVerde]:
        logger.info(
            f"A iniciar download de recibos verdes para {empresa_autenticada.nome} - {ano}/{mes:02d}"
        )
        driver.get("https://irs.portaldasfinancas.gov.pt/recibos/portal/")
        time.sleep(2)
        driver.get("https://irs.portaldasfinancas.gov.pt/recibos/portal/")
        time.sleep(2)
        dia_inicial = 1

        if mes == 2:
            if ano % 4 == 0 and (ano % 100 != 0 or ano % 400 == 0):
                dia_final = 29
            else:
                dia_final = 28
        elif mes in [1, 3, 5, 7, 8, 10, 12]:
            dia_final = 31
        else:
            dia_final = 30

        mes_inicial = 1 if mes == -1 else mes
        mes_final = 12 if mes == -1 else mes
        if tipo == TipoReciboVerdePrestOUAdquir.Prestador:
            modo_consulta_str = f"&modoConsulta=Prestador&nifPrestadorServicos={empresa_autenticada.nif}"
        else:
            modo_consulta_str = (
                f"&modoConsulta=Adquirente&nifAdquirente={empresa_autenticada.nif}"
            )
        url = f"view-source:https://irs.portaldasfinancas.gov.pt/recibos/api/obtemDocumentosV2?dataEmissaoFim={ano}-{mes_final}-{dia_final}&dataEmissaoInicio={ano}-{mes_inicial}-{dia_inicial}&isAutoSearchOn=on{modo_consulta_str}&offset=0&tableSize=10000&tipoPesquisa=1"
        driver.get(url)
        time.sleep(2)
        json_str = driver.find_element(By.XPATH, "/html/body/table/tbody/tr/td[2]").text
        # import os
        # with open("test.json", "r") as f:
        #    json_str = f.read()

        json_data = json.loads(json_str)

        logger.info(
            f"Encontrados {len(json_data.get('listaDocumentos', []))} recibos verdes para {empresa_autenticada.nome} - {ano}/{mes:02d}"
        )

        recibos_verdes = []
        for doc in json_data.get("listaDocumentos", []):
            num_doc = str(doc.get("numDocumento"))
            nif_prestador_servicos = str(doc.get("nifPrestadorServicos"))
            detalhesURL = (
                "https://irs.portaldasfinancas.gov.pt/recibos/portal/consultar/detalheV2/"
                + nif_prestador_servicos
                + "/0/"
                + num_doc
            )

            print("A processar recibo verde: " + num_doc)
            logger.info(f"A processar recibo verde: {num_doc}")

            driver.get(detalhesURL)
            time.sleep(1)
            # IMPLEMENTAR O SCRAPER AQUI PARA OBTER OS DADOS DO RECIBO, E CRIAR O OBJETO RECIBO_VERDE, E ADICIONAR À LISTA

            recibo_verde = ReciboVerde()
            # find any element with info-documento" attribute and get its value
            dados = driver.find_element(By.XPATH, "//*[@info-documento]").get_attribute(
                "info-documento"
            )

            json_str = html.unescape(dados)
            dto = json.loads(json_str)

            recibo_verde.tipo_doc = dto.get("tipoDocumento")
            recibo_verde.num_doc = str(dto.get("numDocumento"))
            recibo_verde.nif_prestador_servicos = str(dto.get("nifPrestadorServicos"))
            recibo_verde.nif_adquirente = dto.get("nifAdquirente")
            recibo_verde.nome_prestador = dto.get("nomePrestador")
            recibo_verde.nome_adquirente = dto.get("nomeAdquirente")
            recibo_verde.pais_adquirente = dto.get("paisDescr")
            recibo_verde.data_emissao = datetime.strptime(
                dto.get("dataEmissao"), "%Y-%m-%d"
            )
            recibo_verde.data_prestacao_servico = datetime.strptime(
                dto.get("dataPrestacaoServico"), "%Y-%m-%d"
            )
            recibo_verde.details_url = dto.get("urlNotasCreditoDoDoc")
            recibo_verde.anulado = dto.get("situacaoCod") != "E"

            tipo_documento_codigo = dto.get("tipoDocumentoCodigo")
            if tipo_documento_codigo == "FR":
                recibo_verde.tipo_recibo_verde = "Pagamento"
            elif tipo_documento_codigo == "F":
                recibo_verde.tipo_recibo_verde = "Fatura"
            elif tipo_documento_codigo == "R":
                recibo_verde.tipo_recibo_verde = "Recibo"
            else:
                recibo_verde.tipo_recibo_verde = "Pagamento"

            recibo_verde.tipo = tipo
            for linha in dto.get("linhasMapeadas", []):
                if linha.get("descricao"):
                    recibo_verde.descricao = linha.get("descricao")
                    break

            recibo_verde.valores = ReciboVerdeValores(
                valor_base=dto.get("valorBase", 0),
                valor_iva_continente=dto.get("valorIVA", 0),
                imposto_selo=dto.get("valorImpostoSelo", 0),
                irs_sem_retencao=dto.get("valorIRS", 0),
                importancia_recebida=dto.get("importanciaRecebida", 0),
            )

            recibos_verdes.append(recibo_verde)

        return recibos_verdes
